﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace MP___algorithms
{
    class main
    {
        static public Dictionary<string, Type> commands = new Dictionary<string, Type>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hola mundo!\nCargando comandos ...");

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in typelist)
            {
                if (type.FullName.Contains("commands"))
                {
                    if (type.GetField("name") != null)
                    {
                        commands.Add(type.GetField("name").GetValue(null).ToString(), type);
                    }
                    else
                    {
                        commands.Add(type.Name, type);
                    }
                }
            }

            Console.WriteLine("¡He cargado {0} comandos!", commands.Count);

            Console.WriteLine("¿Qué te gustaría que haga hoy? (? : ayuda )");
            while (true)
            {
                string answer = Console.ReadLine().ToLower();

                if (commands.ContainsKey(answer))
                {
                    commands[answer].GetMethod("run").Invoke(null, null);
                }
                else
                {
                    Console.WriteLine("No entendí del todo, intente de nuevo...");
                }
            }
        }
    }
}
