using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class TelegramBotHelper
    {
        private const string Text_1 = "Расписание";
        private const string Text_Structure = "Структура";
        private const string Text_Stipend = "Стипендия";
        private const string Text_Events = "Мероприятия";
        private const string Text_5 = "Отзыв";
        private const string Text_6 = "Поддержка";
        private const string Text_Back = "Назад";

        private const string Text_Structure_Decan = "Деканаты";


        private string _token;
        Telegram.Bot.TelegramBotClient _client;
        private Dictionary<long,UserState> _clientStates= new Dictionary<long, UserState>();

        public TelegramBotHelper(string token)
        {
            this._token = token;
        }

        internal void GetUpdates()
        {
            _client = new Telegram.Bot.TelegramBotClient(_token);
            var me=_client.GetMeAsync().Result;
            if(me!= null && !string.IsNullOrEmpty(me.Username))
            {
                int offset = 0;
                while (true)
                {
                    try
                    {
                        
                        var updates = _client.GetUpdatesAsync(offset).Result;
                        if(updates!= null && updates.Count() > 0)
                        {
                            foreach(var update in updates)
                            {
                                processUpdate(update);
                                offset = update.Id + 1;
                            }
                        }
                    }
                    catch (Exception ex){ Console.WriteLine("Error: " + ex.Message); }

                    Thread.Sleep(1000);
                }
            }
            
        }

        private void processUpdate(Telegram.Bot.Types.Update update)
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    var text = update.Message.Text;
                    var state = _clientStates.ContainsKey(update.Message.Chat.Id) ? _clientStates[update.Message.Chat.Id] : null;
                    if (state != null)
                    {
                        switch (state.State)
                        {
                            case State.Structure:
                                if (text.Equals(Text_Back))
                                {
                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите из возможных вариантов", replyMarkup: GetButtons());
                                    _clientStates[update.Message.Chat.Id] = null;
                                }
                                else
                                {
                                    switch (text)
                                    {
                                    case "Деканаты":
                                            if (text.Equals(Text_Back))
                                            {

                                                _client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите из возможных вариантов", replyMarkup: GetStructureButtons());
                                                _clientStates[update.Message.Chat.Id] = new UserState { State = State.Structure };
                                            }
                                            else
                                            {
                                                state.State = State.Direcrorate;
                                                _client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите институт: ", replyMarkup: GetInstituteButtons());
                                               
                                            }
                                            break;
                                    }
                                }
                                break;
                            case State.Direcrorate:
                                if (text.Equals(Text_Back))
                                {

                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите из возможных вариантов", replyMarkup: GetStructureButtons());
                                    _clientStates[update.Message.Chat.Id] = new UserState { State = State.Structure };
                                }
                                else
                                {
                                    var instituteinfo = GetInstitute(text);
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, instituteinfo, replyMarkup: GetInstituteButtons());
                                }
                                break;
                            
                        }
                    }
                    else
                    {
                        switch (text)
                        {
                            case Text_Structure:
                                _clientStates[update.Message.Chat.Id] = new UserState { State = State.Structure };
                               
                                _client.SendTextMessageAsync(update.Message.Chat.Id, "Выберите из возможных вариантов", replyMarkup: GetStructureButtons());
                                break;
                            case Text_Events:
                                //_clientStates[update.Message.Chat.Id] = new UserState { State = State.Structure };

                                _client.SendTextMessageAsync(update.Message.Chat.Id, "Мероприятия", replyMarkup: GetButtons());
                                break;
                            default:
                                _client.SendTextMessageAsync(update.Message.Chat.Id, " Привет, я чат-бот для помощи студентам ИРНИТУ, у меня ты можешь спросить где и как тебе провести время и когда у тебя занятия. "
                                    + "\n Расписание - в этом  разделе вы можете посмотреть расписание своих пар"
                                    + "\n Структура - узнать время работы и место нахождения интересующих вас мест"
                                    + "\n Стипендия - этот раздел раскажет о том, что такое рейтинговая стипендия и как ее получить"
                                    + "\n Мероприятия - здесь можно посмотреть, когда будет ближайший концерт или праздник"
                                    + "\n Отзыв - можете оставить свой отзыв про бота, мы будем рады каждому"
                                    + "\n Поддержка - здесь вы можете написать нам любой вопрос и мы ответим на него в течении 36 часов",  replyMarkup: GetButtons());
                                break;
                        }
                    }
                    //_client.SendTextMessageAsync(update.Message.Chat.Id, "Text"+text, replyMarkup: GetButtons());
                    break;
                default:
                    Console.WriteLine(update.Type + "Not implemented!");
                    break;
            }
        }

        private string GetInstitute(string text)
        {
            //TODO: информация об дирекциях институтов из базы
            if (text.Equals("ИИТиАД"))
            {
                return "Деканат Института информационных технологий и анализа данных \n Офис: В - 210 \n Тел.: +7(3952) 40 - 51 - 60 \n E - mail: f05 @ex.istu.edu";
            }
            else
            {
                return null;
            }
        }

        private IReplyMarkup GetInstituteButtons()
        {
            return new ReplyKeyboardMarkup
           (
             new[]
                               {
                                    new[] {
                                        new KeyboardButton("ИАиТ"),
                                        new KeyboardButton("ИАСиД")
                                    } ,
                                    new[] {
                                        new KeyboardButton("ИВТ"),
                                        new KeyboardButton("ИЭУиП")
                                    },
                                    new[] {
                                        new KeyboardButton("ИИТиАД"),
                                        new KeyboardButton("ИН")
                                    },
                                    new[] {
                                        new KeyboardButton("Назад"),
                                        new KeyboardButton("ИЭ")
                                    }
                               }

           );
        }

        private IReplyMarkup GetStructureButtons()
        {
            return new ReplyKeyboardMarkup
            (           
              new[]
                                {
                                    new[] {
                                        new KeyboardButton("Кафе"),
                                        new KeyboardButton("Деканаты")
                                    } ,
                                    new[] {
                                        new KeyboardButton("Коворкинги"),
                                        new KeyboardButton("Копи центры")
                                    },
                                    new[] {
                                        new KeyboardButton("Назад"),
                                        new KeyboardButton("Библиотека")
                                    }
                                }

            );
        }

        private IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            (
              //new List<List<KeyboardButton>>
              //{
              //    new List<KeyboardButton>
              //    {
              //        new KeyboardButton{ Text = Text_1}, new KeyboardButton{ Text = Text_1},
              //    },
              //    new List<KeyboardButton>
              //    {
              //        new KeyboardButton{ Text = Text_2}, new KeyboardButton{ Text = Text_3},
              //    }
              //}
              new[]
                                {
                                    new[] {
                                        new KeyboardButton("Расписание"),
                                        new KeyboardButton("Структура")
                                    } ,
                                    new[] {
                                        new KeyboardButton("Стипендия"),
                                        new KeyboardButton("Мероприятия")
                                    },
                                    new[] {
                                        new KeyboardButton("Отзыв"),
                                        new KeyboardButton("Поддержка")
                                    }
                                }

            );
        }
    }
}