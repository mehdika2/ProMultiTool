using ProMultiTool.PluginBusinnes;
using ProMultiTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMultiTool
{
    class Program
    {
        static List<MenuItem> menuItems;
        static PluginManager pluginManager;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Pro Multi Tool";

            pluginManager = new PluginManager();
            pluginManager.LoadPlugins("Plugins");

            //var items = new string[] { "Hello", "This", "Is the test menu", "Im gonna test it", "So far yea", "Thats awesome i know", "You know this", "is awesome yoo"
            //, "yoo ghag, sup", "sup yoo boye", "hows going on ?", "password manager", "raz", "openssl", "encryption", "signiture/verify"};

            var items = pluginManager.Plugins.Select(i => i.Key.Name).ToArray();

            menuItems = PutMenuItems(items);

            ControlMenu();
        }

        static void ControlMenu()
        {
            int oldItem = 0;
            int selectedItem = 0;
            string command = string.Empty;
            while (true)
            {
                DrawMenuItems();

                SelectItem(oldItem, selectedItem);

                Console.CursorVisible = false;

                ConsoleKeyInfo key;
                if (command != "")
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(command.Length + 11, Console.WindowHeight - 1);
                    key = Console.ReadKey();
                }
                else key = Console.ReadKey(true);

                if (key.KeyChar == '/')
                {
                    command = "/";
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(0, Console.WindowHeight - 1);
                    Console.Write(" command: /          ");
                    Console.CursorLeft -= 10;
                }
                else if(key.Key == ConsoleKey.Enter)
                {
                    ExecuteCommand(command, ref oldItem, ref selectedItem);
                    command = string.Empty;
                }
                else if (char.IsNumber(key.KeyChar) && command == string.Empty && menuItems.Count < 10)
                {
                    int newSelect = (int)char.GetNumericValue(key.KeyChar) - 1;
                    if (newSelect >= menuItems.Count)
                        continue;
                    oldItem = selectedItem;
                    selectedItem = newSelect;
                    SelectItem(oldItem, selectedItem);
                }
                else if (command != string.Empty)
                {
                    command += key.KeyChar;
                }
                else
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            if (menuItems[selectedItem].Top != 0)
                            {
                                var newItem = menuItems.Where(i => i.Top == menuItems[selectedItem].Top - 2).OrderBy(x => Math.Abs(x.Left - x.Text.Length / 2 - menuItems[selectedItem].Left + menuItems[selectedItem].Text.Length / 2)).First();
                                oldItem = selectedItem;
                                selectedItem = menuItems.IndexOf(newItem);
                            }
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            if (menuItems[selectedItem].Top < menuItems.OrderByDescending(i => i.Top).First().Top)
                            {
                                var newItem = menuItems.Where(i => i.Top == menuItems[selectedItem].Top + 2).OrderBy(i => Math.Abs(i.Left - i.Text.Length / 2 - menuItems[selectedItem].Left + menuItems[selectedItem].PrefixLength + menuItems[selectedItem].Text.Length / 2)).First();
                                oldItem = selectedItem;
                                selectedItem = menuItems.IndexOf(newItem);
                            }
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            if (selectedItem + 1 < menuItems.Count)
                            {
                                oldItem = selectedItem;
                                selectedItem++;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            if (selectedItem - 1 >= 0)
                            {
                                oldItem = selectedItem;
                                selectedItem--;
                            }
                            break;
                    }
            }
        }

        private static void ExecuteCommand(string command, ref int oldItem, ref int selectedItem)
        {
            if (command == string.Empty)
                ExecutePlugin(ref oldItem, ref selectedItem, selectedItem);
            else
            {
                command = command.TrimStart('/');
                int changedSelecte;
                if (int.TryParse(command, out changedSelecte) && changedSelecte <= menuItems.Count && changedSelecte > 0)
                    ExecutePlugin(ref oldItem, ref selectedItem, changedSelecte - 1);
                else 
                {
                    var builtInCommand = pluginManager.Commands.FirstOrDefault(i => i.Name.ToLower() == command.ToLower());
                    if (builtInCommand == null)
                    {
                        // clear typing number if changed method to number to move
                        Console.SetCursorPosition(0, Console.WindowHeight - 1);
                        Console.ResetColor();
                        Console.Write(new string(' ', Console.WindowWidth - 1));
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.CursorLeft = 0;
                        Console.Write(" Unknown command ");
                        Console.ResetColor();
                        command = "";
                    }
                    else
                    {
                        Console.ResetColor();
                        Console.Clear();
                        builtInCommand.Execute();
                        Console.ResetColor();
                        Console.Clear();
                    }
                }
            }
        }

        private static void ExecutePlugin(ref int oldItem, ref int selectedItem, int pluginIndex)
        {
            oldItem = selectedItem;
            selectedItem = pluginIndex;
            Console.ResetColor();
            Console.Clear();
            pluginManager.Plugins.ElementAt(selectedItem).Key.Run();
            Console.ResetColor();
            Console.Clear();
        }

        static void SelectItem(int oldItem, int newItem)
        {
            if (oldItem != newItem)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(menuItems[oldItem].Left, menuItems[oldItem].Top);
                Console.Write(string.Format(" {0}{1} ", (menuItems[oldItem].Index + 1) + ".", menuItems[oldItem].Text));
            }

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.SetCursorPosition(menuItems[newItem].Left, menuItems[newItem].Top);
            Console.Write(string.Format(" {0}{1} ", (menuItems[newItem].Index + 1) + ".", menuItems[newItem].Text));
        }

        static void DrawMenuItems()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            foreach (var item in menuItems)
            {
                Console.SetCursorPosition(item.Left, item.Top);
                Console.Write(string.Format(" {0}{1} ", (item.Index + 1) + ".", item.Text));
            }
        }

        static List<MenuItem> PutMenuItems(string[] menuItems)
        {
            const int spaceBetween = 3;

            List<MenuItem> newItems = new List<MenuItem>();
            int left = 0;
            int top = 1;
            int index = 0;
            foreach (var item in menuItems)
            {
                int preffixLenght = index.ToString().Length + 2;
                if (item.Length + spaceBetween * 2 + 1 >= Console.BufferWidth)
                    throw new Exception("Large item detected!");

                left += spaceBetween + 1;
                if (left + item.Length + spaceBetween > Console.BufferWidth)
                {
                    top += 2;
                    left = spaceBetween + 1;
                }

                newItems.Add(new MenuItem(index, left - 1, top, item, preffixLenght));

                left += item.Length + preffixLenght;
                index++;
            }

            return newItems;
        }
    }
}
