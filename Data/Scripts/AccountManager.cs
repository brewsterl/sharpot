﻿using SharpOT;
using SharpOT.Scripting;
using System.Collections.Generic;

public class AccountManager:IScript
{
    Game game;
    Dictionary<Connection, DialogueState> managers = new Dictionary<Connection, DialogueState>();
    public bool Start(Game game)
    {
        this.game = game;
        game.BeforeCreatureSpeech += BeforeCreatureSpeech;
        return true;
    }

    public bool BeforeCreatureSpeech(Creature creature, Speech speech)
    {
        if (creature.IsPlayer && creature.Name == "Account Manager")
        {
            Server.Log("IF1");
            Player p = (Player)creature;
            if (managers.ContainsKey(p.Connection))
            {
                Parse(p.Connection, managers[p.Connection], speech);
            }
            else
            {
                managers.Add(p.Connection, DialogueState.New);
                Parse(p.Connection, managers[p.Connection], speech);
            }

            Server.Log("ret false");
            return false;
        }

        Server.Log("ret true");
        return true;

    }

    private void Parse(Connection connection,DialogueState state, Speech speech)
    {
        switch (state)
        {
            case DialogueState.New:
                connection.SendTextMessage(TextMessageType.ConsoleBlue, "Hi! What would you like to do? You can \"create\" or \"manage\" an account.");
                managers[connection] = DialogueState.Hi;
                break;
            case DialogueState.Hi:
                switch (speech.Message.ToLower())
                {
                    case "create":
                        connection.SendTextMessage(TextMessageType.ConsoleBlue, "What would you like your account name to be?");
                        managers[connection] = DialogueState.CreateAccountAskName;
                        break;
                    case "manage":
                        connection.SendTextMessage(TextMessageType.ConsoleBlue, "What would you like your account name to be?");
                        managers[connection] = DialogueState.ManageAccountAskName;
                        break;
                    default:
                        connection.SendTextMessage(TextMessageType.ConsoleRed, "Oops! I can't understand what you mean. Let's start again?");
                        connection.SendTextMessage(TextMessageType.ConsoleBlue, "What would you like to do? You can \"create\" or \"manage\" an account.");
                        managers[connection] = DialogueState.ManageAccountAskName;
                        break;                        
                }
                break;

        }
        Server.Log("end parse");
    }

    public bool Stop()
    {
        game.BeforeCreatureSpeech -= BeforeCreatureSpeech;
        return true;
    }

    enum DialogueState
    {
        New,
        Hi,
        
        CreateAccount,
        ManageAccount,

        CreateAccountAskName,
        CreateAccountAskPassword,

        ManageAccountAskName,
        ManageAccountAskPassword,
        //others to add
    }
}