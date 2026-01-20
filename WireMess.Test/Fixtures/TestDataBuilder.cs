using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMess.Models.Entities;
using WireMess.Models.Enums;

namespace WireMess.Test.Fixtures
{
    public class TestDataBuilder
    {
        private readonly Faker _faker = new Faker();

        public User CreateUser(bool isOnline = true)
        {
            return new User()
            {
                Id = _faker.Random.Int(1, 10000),
                Username = _faker.Internet.UserName(),
                Email = _faker.Internet.Email(),
                PasswordHash = _faker.Random.Hash(),
                PasswordSalt = _faker.Random.Hash(),
                IsOnline = isOnline,
                LastActive = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public Message CreateMessage(int senderId, int conversationId, string content = null)
        {
            return new Message
            {
                Id = _faker.Random.Int(1, 10000),
                Content = content ?? _faker.Lorem.Sentence(),
                SenderId = senderId,
                ConversationId = conversationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public Conversation CreateConversation(int typeId = (int)ConversationTypeEnum.Direct)
        {
            return new Conversation
            {
                Id = _faker.Random.Int(1, 10000),
                ConversationName = _faker.Lorem.Word(),
                TypeId = typeId,
                LastMessageAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
