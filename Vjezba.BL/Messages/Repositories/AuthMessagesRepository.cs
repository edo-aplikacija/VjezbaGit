using Vjezba.BL.Messages.Models;
using Vjezba.BL.User.Repositories;
using Vjezba.DL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vjezba.BL.Messages.Repositories
{
    public class AuthMessagesRepository : IDisposable
    {
        private BizHubEntities ctx = new BizHubEntities();

        private AuthUserRepository authRepo = new AuthUserRepository();

        // new message
        public Message SaveNewMessage(NewMessageModel model, int senderId, int recipientId)
        {
                var newMessage = NewMessageModelToMesssage(model, senderId, recipientId);
                ctx.Message.Add(newMessage);
                ctx.SaveChanges();
                SendEmailNotification(model.Recipient);
                return newMessage;
        }

        // saving uploaded file to database
        public void SaveMessageUploadedFile(int messageId, int userId, string fileName, string fileType)
        {
            MessageUploadFilesPath baseFile = new MessageUploadFilesPath();

            // path for database
            string pathToFile = baseFile.pathToMessageUploadFolder + messageId.ToString() + "/" + fileName;

            DateTime myDateTime = DateTime.Now;
            Repository fileRepo = new Repository()
            {
                Name = fileName,
                Type = fileType,
                Created = myDateTime,
                Path = pathToFile
            };
            ctx.Repository.Add(fileRepo);
            ctx.SaveChanges();

            MessageAttachment messageAtch = new MessageAttachment()
            {
                MessageID = messageId,
                ProfileID = userId,
                RepositryItemID = fileRepo.RepositoryItemID
            };
            ctx.MessageAttachment.Add(messageAtch);
            ctx.SaveChanges();
        }

        // reply message
        public Message SaveReplyMessage(ReplyMessageModel model, int userId, string recipientEmail)
        {
                var newMessage = ReplyMessageToMessage(model, userId);
                ctx.Message.Add(newMessage);
                ctx.SaveChanges();
                SendEmailNotification(recipientEmail);
                return newMessage;
        }

        public UnreadedMessagesReturnModel GetUserUnreadedMessages(int userId)
        {
            int totalUnreadedMessages = (from msg in ctx.Message
                                         join user in ctx.Profile on msg.SenderID equals user.ProfileID
                                         where msg.RecipientID == userId && msg.RecipientDeleted == false && msg.ReadAt == null
                                         select msg.MessageID).Count();

            UnreadedMessagesReturnModel unreadedMessages = new UnreadedMessagesReturnModel()
            {
                UnreadedMessages = totalUnreadedMessages
            };

            return unreadedMessages;
        }

        // inbox outbox deleted
        public IQueryable<MessageReturnModel> GetMessagesByStatus(string status, string searchText, int page, int userId)
        {
            // change this value in production to desired value
            // also on frontend /app/authProfileMessages/controllers/authProfileMessagesCtrl.js $scope.messagesPerPage 
            // and /app/authProfileMessages/views/authProfileListMessages.html itemsPerPage
            int pageLength = 10;

            int pageToSkip = (page - 1) * pageLength;
            

            if (status == "Inbox")
            {
                int totalUnreadedMessages = (from msg in ctx.Message
                                     join user in ctx.Profile on msg.SenderID equals user.ProfileID
                                             where msg.RecipientID == userId && msg.RecipientDeleted == false && msg.ReadAt == null
                                     select msg.MessageID).Count();

                int totalMessages = (from msg in ctx.Message
                                     join user in ctx.Profile on msg.SenderID equals user.ProfileID
                                     where msg.RecipientID == userId && msg.RecipientDeleted == false && user.Email.Contains(searchText)
                                     select msg.MessageID).Count();
              
                if (totalMessages < 1)
                {
                    return null;
                }

                var result = (from msg in ctx.Message
                              join user in ctx.Profile on msg.SenderID equals user.ProfileID
                              where msg.RecipientID == userId && msg.RecipientDeleted == false && user.Email.Contains(searchText)
                              orderby msg.Created descending
                             
                              select new MessageReturnModel
                              {
                                  MessageID = msg.MessageID,
                                  SenderID = msg.SenderID,
                                  RecipientID = msg.RecipientID,
                                  Subject = msg.Subject,
                                  Body = msg.Body,
                                  Created = msg.Created,
                                  ReadAt = msg.ReadAt,
                                  SenderDeleted = msg.SenderDeleted,
                                  RecipientDeleted = msg.RecipientDeleted,
                                  ConversationID = msg.ConversationID,
                                  ProfileID = user.ProfileID,
                                  Email = user.Email,
                                  TotalMessages = totalMessages,
                                  UnreadedMessages = totalUnreadedMessages
                              }
                                ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }
            else if (status == "Outbox")
            {
                int totalMessages = (from msg in ctx.Message
                                     join user in ctx.Profile on msg.RecipientID equals user.ProfileID
                                     where msg.SenderID == userId && msg.SenderDeleted == false && user.Email.Contains(searchText)
                                     select msg.MessageID).Count();

                if (totalMessages < 1)
                {
                    return null;
                }

                var result = (from msg in ctx.Message
                              join user in ctx.Profile on msg.RecipientID equals user.ProfileID
                              where msg.SenderID == userId && msg.SenderDeleted == false && user.Email.Contains(searchText)
                              orderby msg.Created descending

                              select new MessageReturnModel
                              {
                                  MessageID = msg.MessageID,
                                  SenderID = msg.SenderID,
                                  RecipientID = msg.RecipientID,
                                  Subject = msg.Subject,
                                  Body = msg.Body,
                                  Created = msg.Created,
                                  ReadAt = msg.ReadAt,
                                  SenderDeleted = msg.SenderDeleted,
                                  RecipientDeleted = msg.RecipientDeleted,
                                  ConversationID = msg.ConversationID,
                                  ProfileID = user.ProfileID,
                                  Email = user.Email,
                                  TotalMessages = totalMessages,
                                  UnreadedMessages = -1
                              }
                                ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }
            else if (status == "Deleted")
            {
                int totalMessages = (from msg in ctx.Message
                                     join user in ctx.Profile on msg.SenderID equals user.ProfileID
                                     where (msg.SenderID == userId && msg.SenderDeleted == true) || (msg.RecipientID == userId && msg.RecipientDeleted == true) && user.Email.Contains(searchText)
                                     select msg.MessageID).Count();

                if (totalMessages < 1)
                {
                    return null;
                }

                var result = (from msg in ctx.Message
                              join user in ctx.Profile on msg.SenderID equals user.ProfileID
                              where ((msg.SenderID == userId && msg.SenderDeleted == true) || (msg.RecipientID == userId && msg.RecipientDeleted == true)) && user.Email.Contains(searchText)
                              orderby msg.Created descending

                              select new MessageReturnModel
                              {
                                  MessageID = msg.MessageID,
                                  SenderID = msg.SenderID,
                                  RecipientID = msg.RecipientID,
                                  Subject = msg.Subject,
                                  Body = msg.Body,
                                  Created = msg.Created,
                                  ReadAt = msg.ReadAt,
                                  SenderDeleted = msg.SenderDeleted,
                                  RecipientDeleted = msg.RecipientDeleted,
                                  ConversationID = msg.ConversationID,
                                  ProfileID = user.ProfileID,
                                  Email = user.Email,
                                  TotalMessages = totalMessages,
                                  UnreadedMessages = -1
                              }
                                ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }
            else
            {
                return null;
            }
            
        }

        // preview message
        public MessagePreviewModel GetMessagePreview(int messageId, int userId) {
            var result = (from msg in ctx.Message
                          join user in ctx.Profile on msg.SenderID equals user.ProfileID                   
                          where (msg.MessageID == messageId) && (msg.SenderID == userId || msg.RecipientID == userId)

                          select new MessagePreviewModel
                          {
                              MessageID = msg.MessageID,
                              SenderID = msg.SenderID,
                              RecipientID = msg.RecipientID,
                              Subject = msg.Subject,
                              Body = msg.Body,
                              Created = msg.Created,
                              ReadAt = msg.ReadAt,
                              SenderDeleted = msg.SenderDeleted,
                              RecipientDeleted = msg.RecipientDeleted,
                              ConversationID = msg.ConversationID,
                              ProfileID = user.ProfileID,
                              Email = user.Email,
                              Name = user.Name,
                              ProfilePicture = user.ProfilePicture,
                              Attachments = (from repo in ctx.Repository
                                             join msgAtch in ctx.MessageAttachment on repo.RepositoryItemID equals msgAtch.RepositryItemID
                                             where msgAtch.MessageID == msg.MessageID
                                             select new MessageAttachReturnModel 
                                             {
                                                RepositoryItemID = repo.RepositoryItemID,
                                                Type = repo.Type,
                                                Name = repo.Name,
                                                Path = repo.Path
                                             }).ToList()
                          }
                                ).First();
            return result;
        }

        // when uploading message files check
        public bool CheckIfMessageBelongToUser(int messageId, int userId)
        {
            var result = ctx.Message.FirstOrDefault(m => (m.MessageID == messageId) && (m.SenderID == userId));
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // preview message and related messages
        public IQueryable<MessageConversationModel> GetMessageConversation(int messageId, int userId)
        {
            var originalMessage = ctx.Message.FirstOrDefault(r => r.MessageID == messageId);
            if (originalMessage == null)
            {
                return null;
            }
            else if (originalMessage.SenderID != userId && originalMessage.RecipientID != userId)
            {
                return null;
            }
            else if (originalMessage.ConversationID == null)
            {
                var result = (from msg in ctx.Message
                              join user in ctx.Profile on msg.SenderID equals user.ProfileID 
                              where (msg.MessageID == originalMessage.MessageID) || (msg.ConversationID == originalMessage.MessageID)
                              orderby msg.Created descending
                              select new MessageConversationModel
                              {
                                  MessageID = msg.MessageID,
                                  SenderID = msg.SenderID,
                                  RecipientID = msg.RecipientID,
                                  Subject = msg.Subject,
                                  Body = msg.Body,
                                  Created = msg.Created,
                                  ReadAt = msg.ReadAt,
                                  SenderDeleted = msg.SenderDeleted,
                                  RecipientDeleted = msg.RecipientDeleted,
                                  ConversationID = msg.ConversationID,
                                  ProfilePicture = user.ProfilePicture,
                                  Name = user.Name,
                                  Email = user.Email,
                                  Attachments = (from repo in ctx.Repository
                                                 join msgAtch in ctx.MessageAttachment on repo.RepositoryItemID equals msgAtch.RepositryItemID
                                                 where msgAtch.MessageID == msg.MessageID
                                                 select new MessageAttachReturnModel
                                                 {
                                                     RepositoryItemID = repo.RepositoryItemID,
                                                     Type = repo.Type,
                                                     Name = repo.Name,
                                                     Path = repo.Path
                                                 }).ToList()
                              }
                                );               

                return result.AsQueryable();
            }
            else 
            {
                var result = (from msg in ctx.Message
                              join user in ctx.Profile on msg.SenderID equals user.ProfileID 
                              where (msg.ConversationID == originalMessage.ConversationID) || (msg.MessageID == originalMessage.ConversationID)
                              orderby msg.Created descending
                              select new MessageConversationModel
                              {
                                  MessageID = msg.MessageID,
                                  SenderID = msg.SenderID,
                                  RecipientID = msg.RecipientID,
                                  Subject = msg.Subject,
                                  Body = msg.Body,
                                  Created = msg.Created,
                                  ReadAt = msg.ReadAt,
                                  SenderDeleted = msg.SenderDeleted,
                                  RecipientDeleted = msg.RecipientDeleted,
                                  ConversationID = msg.ConversationID,
                                  ProfilePicture = user.ProfilePicture,
                                  Name = user.Name,
                                  Email = user.Email,
                                  Attachments = (from repo in ctx.Repository
                                                 join msgAtch in ctx.MessageAttachment on repo.RepositoryItemID equals msgAtch.RepositryItemID
                                                 where msgAtch.MessageID == msg.MessageID
                                                 select new MessageAttachReturnModel
                                                 {
                                                     RepositoryItemID = repo.RepositoryItemID,
                                                     Type = repo.Type,
                                                     Name = repo.Name,
                                                     Path = repo.Path
                                                 }).ToList()
                              }
                                );
               
                return result.AsQueryable();
            }
        }

        // delete and undelete
        public bool UpdateMessageStatus(int[] deletedMessages, int userId, bool senderDeleted, bool recipientDeleted)
        {
            if (deletedMessages.Length < 1)
            {
                return false;
            }

            foreach (int messageId in deletedMessages)
            {
                var result = ctx.Message.FirstOrDefault(r => r.MessageID == messageId);
                if (result == null)
                {
                    return false;
                }
                else if (result.SenderID != userId && result.RecipientID != userId)
                {
                    return false;
                }
                else
                {
                    if (result.SenderID == userId)
                    {
                        result.SenderDeleted = senderDeleted;
                    }
                    else
                    {
                        result.RecipientDeleted = recipientDeleted;
                    }

                    ctx.Entry(result).State = EntityState.Modified;
                    
                    
                }
            }
            ctx.SaveChanges();
            return true;
        }

        public bool UpdateMessageReadAt(int messageId, int userId)
        {
            var result = ctx.Message.FirstOrDefault(r => r.MessageID == messageId);
            if (result == null || result.RecipientID != userId)
            {
                return false;
            }
            else
            {
                DateTime myDateTime = DateTime.Now;
                result.ReadAt = myDateTime;
                ctx.Entry(result).State = EntityState.Modified;
                ctx.SaveChanges();
                return true;
            }
        }

        // helper methods

        private Message NewMessageModelToMesssage(NewMessageModel model, int senderId, int recipientId)
        {
            DateTime myDateTime = DateTime.Now;
            Message newMessage = new Message()
            {
                SenderID = senderId,
                RecipientID = recipientId,
                Subject = model.Subject,
                Body = model.Body,
                Created = myDateTime,
                ReadAt = null,
                SenderDeleted = false,
                RecipientDeleted = false,
                ConversationID = null
            };

            return newMessage;
        }

        private Message ReplyMessageToMessage(ReplyMessageModel model, int senderId)
        {
            DateTime myDateTime = DateTime.Now;
            Message newMessage = new Message()
            {
                SenderID = senderId,
                RecipientID = model.RecipientId,
                Subject = model.Subject,
                Body = model.Body,
                Created = myDateTime,
                ReadAt = null,
                SenderDeleted = false,
                RecipientDeleted = false,
                ConversationID = model.ConversationId
            };

            return newMessage;
        }

        private void SendEmailNotification(string userEmail)
        {
            SendEmailConf emailConf = new SendEmailConf();
            MailMessage msg = new MailMessage();

            msg.Subject = "Nova Poruka";

            string newLine = ("<br/>");
            string userNameMsg = "Dobili ste novu poruku " + newLine + newLine;
            string websiteMsg = "Prijavite se na www.bizhub.ba i provjerite nove poruke." + newLine;

            string messageToSend = "Poštovani," + newLine + newLine + userNameMsg + websiteMsg;
            msg.Body = messageToSend;

            msg.From = new MailAddress("bizhub.mail.service@gmail.com");
            msg.To.Add(userEmail);
            msg.IsBodyHtml = true;
            try
            {
                emailConf.smtp.Send(msg);

            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
                return;
            }
        }

        public void Dispose()
        {
            ctx.Dispose();
        }
    }
}
