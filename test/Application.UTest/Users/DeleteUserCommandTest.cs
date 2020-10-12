using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crpg.Application.Common.Results;
using Crpg.Application.Users.Commands;
using Crpg.Domain.Entities;
using Crpg.Sdk.Abstractions.Events;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Users
{
    public class DeleteUserCommandTest : TestBase
    {
        [Test]
        public async Task DeleteExistingUser()
        {
            var user = ArrangeDb.Users.Add(new User
            {
                Characters = new List<Character> { new Character() },
                OwnedItems = new List<UserItem> { new UserItem { Item = new Item() } },
                Bans = new List<Ban> { new Ban() }
            });
            await ArrangeDb.SaveChangesAsync();

            // needs to be saved before UserItems[0] gets deleted
            int itemId = user.Entity.OwnedItems[0].ItemId;

            await new DeleteUserCommand.Handler(ActDb, Mock.Of<IEventRaiser>()).Handle(new DeleteUserCommand
            {
                UserId = user.Entity.Id
            }, CancellationToken.None);

            Assert.ThrowsAsync<InvalidOperationException>(() => AssertDb.Characters.FirstAsync(c => c.Id == user.Entity.Characters[0].Id));
            Assert.ThrowsAsync<InvalidOperationException>(() => AssertDb.UserItems.FirstAsync(oi =>
                oi.UserId == user.Entity.Id && oi.ItemId == user.Entity.OwnedItems[0].ItemId));
            Assert.DoesNotThrowAsync(() => AssertDb.Users.FirstAsync(u => u.Id == user.Entity.Id));
            Assert.DoesNotThrowAsync(() => AssertDb.Items.FirstAsync(i => i.Id == itemId));
            Assert.DoesNotThrowAsync(() => AssertDb.Bans.FirstAsync(b => b.BannedUserId == user.Entity.Id));
        }

        [Test]
        public async Task DeleteNonExistingUser()
        {
            var result =
                await new DeleteUserCommand.Handler(ActDb, Mock.Of<IEventRaiser>()).Handle(
                    new DeleteUserCommand { UserId = 1 }, CancellationToken.None);
            Assert.AreEqual(ErrorCode.UserNotFound, result.Errors![0].Code);
        }
    }
}
