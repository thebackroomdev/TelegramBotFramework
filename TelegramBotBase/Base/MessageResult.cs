﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotBase.Base
{
    public class MessageResult : ResultBase
    {
        public Telegram.Bot.Args.MessageEventArgs RawMessageData { get; set; }

        public Telegram.Bot.Args.CallbackQueryEventArgs RawCallbackData { get; set; }

        /// <summary>
        /// Returns the Device/ChatId
        /// </summary>
        public override long DeviceId
        {
            get
            {
                return this.RawMessageData?.Message?.Chat.Id ?? this.RawCallbackData?.CallbackQuery.Message?.Chat.Id ?? 0;
            }
        }

        /// <summary>
        /// The message id
        /// </summary>
        public new int MessageId
        {
            get
            {
                return this.Message?.MessageId ?? this.RawCallbackData?.CallbackQuery?.Message?.MessageId ?? 0;
            }
        }

        public String Command
        {
            get
            {
                return this.RawMessageData?.Message?.Text ?? "";
            }
        }

        public String MessageText
        {
            get
            {
                return this.RawMessageData?.Message?.Text ?? "";
            }
        }

        /// <summary>
        /// Is this an action ? (i.e. button click)
        /// </summary>
        public bool IsAction
        {
            get
            {
                return (this.RawCallbackData != null);
            }
        }

        /// <summary>
        /// Is this a system call ? Starts with a slash '/' and a command
        /// </summary>
        public bool IsSystemCall
        {
            get
            {
                return (this.MessageText.StartsWith("/"));
            }
        }

        /// <summary>
        /// Returns a List of all parameters which has been sent with the command itself (i.e. /start 123 456 789 => 123,456,789)
        /// </summary>
        public List<String> SystemCallParameters
        {
            get
            {
                if (!IsSystemCall)
                    return new List<string>();

                //Split by empty space and skip first entry (command itself), return as list
                return this.MessageText.Split(' ').Skip(1).ToList();
            }
        }

        /// <summary>
        /// Returns just the System call command (i.e. /start 1 2 3 => /start)
        /// </summary>
        public String SystemCommand
        {
            get
            {
                if (!IsSystemCall)
                    return null;

                return this.MessageText.Split(' ')[0];
            }
        }

        public bool Handled { get; set; } = false;

        public String RawData
        {
            get
            {
                return this.RawCallbackData?.CallbackQuery?.Data;
            }
        }

        public T GetData<T>()
            where T : class
        {
            T cd = null;
            try
            {
                cd = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(this.RawData);

                return cd;
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// Confirm incomming action (i.e. Button click)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ConfirmAction(String message = "")
        {
            try
            {
                await this.Client.TelegramClient.AnswerCallbackQueryAsync(this.RawCallbackData.CallbackQuery.Id, message);
            }
            catch
            {

            }
        }

        public override async Task DeleteMessage()
        {
            try
            {
                await base.DeleteMessage(this.MessageId);
            }
            catch
            {

            }
        }


        public MessageResult(Telegram.Bot.Args.MessageEventArgs rawdata)
        {
            this.RawMessageData = rawdata;
            this.Message = rawdata.Message;
        }

        public MessageResult(Telegram.Bot.Args.CallbackQueryEventArgs callback)
        {
            this.RawCallbackData = callback;
            this.Message = callback.CallbackQuery.Message;
        }

    }
}
