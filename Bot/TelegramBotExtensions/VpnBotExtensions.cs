﻿using Bot.Models.VpnBot.Commands;
using System;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.VpnBotExtensions
{
    public static class VpnBotExtensions
    {
        public static async void NotifyUsers(this ITelegramBotClient botClient, string newPassword)
        {
            foreach (var line in System.IO.File.ReadAllLines("Files/Chats.txt"))
            {
                long chatId = Convert.ToInt64(line);
                await botClient.SendTextMessageAsync(chatId, "Password changed! New passport:");
                await botClient.SendTextMessageAsync(chatId, $"`{newPassword}`", parseMode: ParseMode.Markdown);
            }
        }

        public async static void ProcessInput(this ITelegramBotClient botClient, Update update)
        {
            if (update == null) return;
            var message = update.Message;
            if (message?.Type == MessageType.TextMessage)
            {
                if (BotTools.TryParseCommand(message.Text, out string command, out string args))
                {
                    if (VpnBotCommandCollection.Includes(command))
                    {
                        botClient.ProcessCommand(message, command, args);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sorry, i don't understand you.");
                }
            }
        }

        public static void ProcessCommand(this ITelegramBotClient botClient, Message update, string command, string args = "")
        {
            VpnBotCommandCollection.GetAllCommands()[command].Execute(botClient, update, args);
        }

        public async static void AddChat(this ITelegramBotClient botClient, long chatId)
        {
            using (var stream = System.IO.File.AppendText("Files/Chats.txt"))
            {
                await stream.WriteLineAsync(chatId.ToString());
            }
        }

        public async static void DeleteChat(this ITelegramBotClient botClient, long chatId)
        {
            var ids = System.IO.File.ReadAllLines("Files/Chats.txt").Where(x => x != chatId.ToString());
            using (var stream = new StreamWriter("Files/Chats.txt", append: false))
            {
                foreach (var line in ids)
                {
                    await stream.WriteLineAsync(line);
                }
            }
        }
    }
}
