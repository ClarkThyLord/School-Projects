﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP___tienda_de_juegos
{
    class main
    {
        public static Random ran = new Random();

        public static string[] names;
        public static string[] descriptions;

        public struct product
        {
            public int _id;
            public string id;
            public string discount_id;
            public string name;
            public string description;
            public int inventory;
            public double cost;
        }

        public struct discount
        {
            public int _id;
            public string id;
            public string product_id;
            public int percentage;
            public double amount;
        }

        public static product[] products;
        public static discount[] discounts;

        static void Main(string[] args)
        {
            // LOAD JSONS
            using (System.IO.StreamReader r = new StreamReader("./db/products/names.json"))
            {
                string json = r.ReadToEnd();
                names = JsonConvert.DeserializeObject<string[]>(json);
            }
            using (System.IO.StreamReader r = new StreamReader("./db/products/descriptions.json"))
            {
                string json = r.ReadToEnd();
                descriptions = JsonConvert.DeserializeObject<string[]>(json);
            }

            while (true)
            {
                header();
                Console.WriteLine("¿Qué te gustaría que haga hoy? (? : ayuda )");

                Console.ForegroundColor = ConsoleColor.Green;
                string answer = Console.ReadLine().ToLower();

                switch (answer)
                {
                    case "?":
                        help();
                        break;
                    case "1":
                        product_list();
                        break;
                    case "2":
                        product_creation();
                        break;
                    case "3":
                        product_creation(false);
                        break;
                    case "4":
                        discount_list();
                        break;
                    case "5":
                        discount_creation();
                        break;
                    case "6":
                        discount_creation(false);
                        break;
                    case "7":
                        shop_open();
                        break;
                    case "8":
                        shop_open(false);
                        break;
                    case "salir":
                        return;
                    default:
                        error("No entendí del todo, intente de nuevo...");
                        Console.ReadKey();
                        continue;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n\nPulse cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        public static void header(ConsoleColor color = ConsoleColor.White)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Christian Moises Cuevas Larin 029794");
            Console.WriteLine("====================================");
            Console.WriteLine("TIENDA DE JUEGOS V1.0");
            Console.WriteLine("************************************");
            Console.ForegroundColor = color;
        }

        public static void error(string error, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = color;
        }

        public static void help()
        {
            header(ConsoleColor.Blue);
            Console.WriteLine("************************************");
            Console.WriteLine("Menú de Ayuda");
            Console.WriteLine("************************************");
            Console.WriteLine("?   :   ayuda");
            Console.WriteLine("");
            Console.WriteLine("OPCIONES DE GESTION");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("1   :   lista de productos");
            Console.WriteLine("2   :   auto crear productos");
            Console.WriteLine("3   :   crear productos manualmente");

            Console.WriteLine("4   :   lista de descuentos");
            Console.WriteLine("5   :   auto crear descuentos");
            Console.WriteLine("6   :   crear descuentos manualmente");
            Console.WriteLine("");
            Console.WriteLine("OPCIONES DE TIENDA");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("7   :   abrir la tienda, y manejar automáticamente");
            Console.WriteLine("8   :   abrir la tienda");
            Console.WriteLine("");
            Console.WriteLine("salir   :   salir del programa");
        }

        public static void product_view(product _product, ConsoleColor color = ConsoleColor.White, bool first = true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ID: {0}\nNombre: {1}\nDescripción: {2}\nCuenta: {3}\nCosto: {4}", _product.id, _product.name, _product.description, _product.inventory, _product.cost);
            
            if (first && _product.discount_id != null && _product.discount_id != "")
            {
                Console.WriteLine("====================");
                Console.WriteLine("      DESCUENTO     ");
                discount_view(discounts.Single(discount => discount.id == _product.discount_id), ConsoleColor.Cyan, !first);
                Console.WriteLine("====================");
            }

            Console.ForegroundColor = color;
        }

        public static void product_list()
        {
            header();

            if (products != null)
            {
                foreach (product _product in products)
                {
                    product_view(_product);
                    Console.WriteLine("---");
                }

                Console.WriteLine("{0} productos totales!", products.Length);
            } else
            {
                error("No hay productos para ver...");
            }
        }

        public static void product_creation(bool auto = true)
        {
            if (discounts != null)
            {
                for (int i = 0; i < discounts.Length; i++)
                {
                    discounts[i] = new discount();
                }
            }

            int max = 0;
            while (true)
            {
                header();
                Console.WriteLine("¿Cuántos productos te gustaría manejar? (25 - 50)");
                while (!int.TryParse(Console.ReadLine(), out max))
                {
                    error("Por favor ingrese un número real, entre 25 y 50...");
                    Console.ReadKey();

                    header();
                    Console.WriteLine("¿Cuántos productos te gustaría manejar? (25 - 50)");
                }

                if (max < 25 || max > 50)
                {
                    error("Por favor ingrese un número real, entre 25 y 50...");
                    Console.ReadKey();
                    continue;
                }

                break;
            }

            products = new product[max];

            string[] used_names = new string[max];
            string[] used_descriptions = new string[max];
            for (int i = 0; i < max; i++)
            {
                products[i]._id = i;
                products[i].id = i.ToString().PadLeft(5, '0');

                if (auto)
                {
                    string name = "";
                    while (true)
                    {
                        name = names[ran.Next(names.Length - 1)];
                        if (used_names.Contains(name))
                        {
                            continue;
                        }
                        used_names[i] = name;
                        break;
                    }
                    products[i].name = name;

                    string description = "";
                    while (true)
                    {
                        description = descriptions[ran.Next(descriptions.Length - 1)];
                        if (used_descriptions.Contains(description))
                        {
                            continue;
                        }
                        used_descriptions[i] = description;
                        break;
                    }
                    products[i].description = description;

                    products[i].inventory = ran.Next(50);
                    products[i].cost = ran.Next(20, 80);
                }
                else
                {
                    while (true)
                    {
                        header();
                        string ans = "";
                        while (true)
                        {
                            Console.WriteLine("Nombre del producto:");
                            ans = Console.ReadLine();

                            if (used_names.Contains(ans))
                            {
                                error("Nombre del producto ya usado, elija otro nombre...");
                                continue;
                            }

                            Console.WriteLine("¿Continuar? (1 : si, 2 : no)");
                            if (Console.ReadLine() == "2")
                            {
                                continue;
                            }
                            break;
                        }
                        used_names[i] = ans;
                        products[i].name = ans;

                        header();

                        while (true)
                        {
                            Console.WriteLine("Descripción del producto:");
                            ans = Console.ReadLine();

                            if (used_descriptions.Contains(ans))
                            {
                                error("Descripción del producto ya utilizado, elija otra descripción...");
                                continue;
                            }

                            Console.WriteLine("¿Continuar? (1 : si, 2 : no)");
                            if (Console.ReadLine() == "2")
                            {
                                continue;
                            }
                            break;
                        }
                        used_descriptions[i] = ans;
                        products[i].description = ans;

                        int inventory = 0;
                        while (true)
                        {
                            header();
                            Console.WriteLine("¿Cuánto inventario del producto?");
                            while (!int.TryParse(Console.ReadLine(), out inventory))
                            {
                                error("Por favor ingrese un número real, mayor a 0...");
                                Console.ReadKey();

                                header();
                                Console.WriteLine("¿Cuánto inventario del producto?");
                            }

                            if (inventory < 1)
                            {
                                error("Por favor ingrese un número real, mayor a 0...");
                                Console.ReadKey();
                                continue;
                            }

                            break;
                        }
                        products[i].inventory = inventory;

                        int cost = 0;
                        while (true)
                        {
                            header();
                            Console.WriteLine("¿Costo del producto?");
                            while (!int.TryParse(Console.ReadLine(), out cost))
                            {
                                error("Por favor ingrese un número real, mayor a 0...");
                                Console.ReadKey();

                                header();
                                Console.WriteLine("¿Costo del producto?");
                            }

                            if (cost < 1)
                            {
                                error("Por favor ingrese un número real, mayor a 0...");
                                Console.ReadKey();
                                continue;
                            }

                            break;
                        }
                        products[i].cost = cost;

                        header();
                        product_view(products[i]);
                        Console.WriteLine("¿Continuar? (1 : si, 2 : no)");
                        if (Console.ReadLine() == "2")
                        {
                            continue;
                        }
                        break;
                    }
                }
            }

            product_list();
            Console.WriteLine("¡Nuevo inventario se ha establecido!");
        }

        public static void discount_view(discount _discount, ConsoleColor color = ConsoleColor.White, bool first = true)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("ID: {0}", _discount.id);

            if (first)
            {
                Console.WriteLine("====================");
                Console.WriteLine("      PRODUCTO      ");
                product_view(products.Single(product => product.id == _discount.product_id), ConsoleColor.DarkCyan, !first);
                Console.WriteLine("====================");
            }

            Console.WriteLine("Porcentaje: %{0} ~ ${1}", _discount.percentage, _discount.amount);
            Console.ForegroundColor = color;
        }

        public static void discount_list()
        {
            header();

            if (discounts != null)
            {
                foreach (discount _discount in discounts)
                {
                    discount_view(_discount);
                    Console.WriteLine("---");
                }

                Console.WriteLine("{0} descuentos totales!", discounts.Length);
            }
            else
            {
                error("No hay descuentos para ver...");
            }
        }

        public static void discount_creation(bool auto = true)
        {
            if (!(products != null))
            {
                error("No hay productos a los que se puedan aplicar descuentos...");
                return;
            }

            for (int i = 0; i < products.Length; i++)
            {
                products[i].discount_id = "";
            }

            int max = 0;
            while (true)
            {
                header();
                Console.WriteLine("¿Cuántos descuentos te gustaría manejar? (1 - 10)");
                while (!int.TryParse(Console.ReadLine(), out max))
                {
                    error("Por favor ingrese un número real, entre 1 y 10...");
                    Console.ReadKey();

                    header();
                    Console.WriteLine("¿Cuántos descuentos te gustaría manejar? (1 - 10)");
                }

                if (max < 1 || max > 10)
                {
                    error("Por favor ingrese un número real, entre 1 y 10...");
                    Console.ReadKey();
                    continue;
                }

                break;
            }

            discounts = new discount[max];

            string[] used_products = new string[max];
            for (int i = 0; i < max; i++)
            {
                discounts[i]._id = i;
                discounts[i].id = i.ToString().PadLeft(5, '0');

                if (auto)
                {
                    product _product;
                    while (true)
                    {
                        _product = products[ran.Next(products.Length - 1)];
                        if (used_products.Contains(_product.id))
                        {
                            continue;
                        }
                        used_products[i] = _product.id;
                        break;
                    }
                    products[_product._id].discount_id = discounts[i].id;
                    discounts[i].product_id = _product.id;

                    discounts[i].percentage = ran.Next(5, 75);
                    discounts[i].amount = _product.cost * (discounts[i].percentage * 0.01);
                }
                else
                {
                    while (true)
                    {
                        header();
                        product _product;
                        while (true)
                        {
                            Console.WriteLine("¿A qué producto le gustaría aplicar un descuento? (id)");
                            string id = Console.ReadLine().PadLeft(5, '0');

                            if (used_products.Contains(id))
                            {
                                error("El producto ya tiene un descuento aplicado...");
                                continue;
                            }

                            _product = products.Single(product => product.id == id);

                            Console.WriteLine("");
                            product_view(_product);
                            Console.WriteLine("");

                            Console.WriteLine("¿Continuar? (1 : si, 2 : no)");
                            if (Console.ReadLine() == "2")
                            {
                                continue;
                            }
                            break;
                        }
                        used_products[i] = _product.id;
                        products[_product._id].discount_id = discounts[i].id;
                        discounts[i].product_id = _product.id;

                        header();

                        int percentage = 0;
                        while (true)
                        {
                            header();
                            Console.WriteLine("¿Cuál es el porcentaje de descuento que se aplica?");
                            while (!int.TryParse(Console.ReadLine(), out percentage))
                            {
                                error("Por favor ingrese un número real, entre 0 y 100...");
                                Console.ReadKey();

                                header();
                                Console.WriteLine("¿Cuál es el porcentaje de descuento que se aplica?");
                            }

                            if (percentage < 1)
                            {
                                error("Por favor ingrese un número real, entre 0 y 100...");
                                Console.ReadKey();
                                continue;
                            }

                            break;
                        }
                        discounts[i].percentage = percentage;
                        discounts[i].amount = _product.cost * (discounts[i].percentage * 0.01);

                        header();
                        discount_view(discounts[i]);
                        Console.WriteLine("¿Continuar? (1 : si, 2 : no)");
                        if (Console.ReadLine() == "2")
                        {
                            continue;
                        }
                        break;
                    }
                }
            }

            discount_list();
            Console.WriteLine("¡Nuevo descuentos se ha establecido!");
        }

        public static void shop_transaction(product[] _products, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            double cash_back = 0;
            double total = 0;

            foreach (product _product in _products)
            {
                Console.WriteLine("Nombre: {0}", _product.name);

                double transaction = _product.cost;

                Console.WriteLine("Precio: {0}", _product.cost);

                if (_product.discount_id != null && _product.discount_id != "")
                {
                    discount _discount = discounts.Single(discount => discount.id == _product.discount_id);
                    transaction -= _discount.amount;
                    cash_back += _discount.amount;

                    Console.WriteLine("Descuento: {0}% ~ ${1}", _discount.percentage, Math.Round(_discount.amount, 2));
                }

                Console.WriteLine("Total: {0}\n", Math.Round(transaction, 2));

                total += transaction;
            }

            Console.WriteLine("Dinero Devuelto: {0}\nTotal: {1}", cash_back, total);

            Console.ForegroundColor = color;
        }

        public static void shop_open(bool auto = true)
        {
            header();

            if (auto)
            {
                int customers = ran.Next(3, 10);
                for (int i = 0; i < customers; i++)
                {
                    int bought = ran.Next(1, 3);
                    product[] _products = new product[bought];
                    for (int n = 0; n < bought; n ++)
                    {
                        product _product = products[ran.Next(products.Length - 1)];

                        if (_product.inventory <= 0)
                        {
                            continue;
                        }

                        products[_product._id].inventory -= 1;
                        _products[n] = _product;
                    }

                    shop_transaction(_products, ConsoleColor.Yellow);
                    Console.WriteLine("---");
                }
            }
            else
            {
                while (true)
                {
                    product[] _products = new product[3];
                    for (int n = 0; n < 3; n++)
                    {
                        header();
                        Console.WriteLine("¡LA TIENDA ESTÁ ABIERTA!");
                        Console.WriteLine("========================");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Bienvenido al cliente de la tienda de videojuegos!");
                        error("¡EL ÚNICO LUGAR EN EL QUE SOLO COMPRAS TRES JUEGOS DE VIDEO DIFERENTES UNA VEZ!", ConsoleColor.Yellow);

                        Console.WriteLine("¿Qué te gustaría comprar? (nombre)");
                        string name = Console.ReadLine();

                        product _product = products.Single(product => product.name.Contains(name));

                        if (_product.inventory <= 0)
                        {
                            error("¡Lo sentimos, no tenemos ese juego en stock!");
                            continue;
                        }

                        Console.WriteLine("");
                        product_view(_product, ConsoleColor.Yellow);
                        Console.WriteLine("");

                        Console.WriteLine("¿Te gustaría comprar? (1 : si, 2 : no)");
                        if (Console.ReadLine() != "2")
                        {
                            products[_product._id].inventory -= 1;
                            _products[n] = _product;

                            Console.WriteLine("Genial, voy a agregar eso a tu carrito de compras...");
                        }

                        if (_products.Length < 3)
                        {
                            Console.WriteLine("¿Seguir comprando? (1 : si, 2 : no)");
                            if (Console.ReadLine() == "1")
                            {
                                continue;
                            }
                        }
                        break;
                    }

                    shop_transaction(_products, ConsoleColor.Yellow);
                    Console.WriteLine("¡Muy bien, esperamos que vuelvas!");
                    Console.ReadKey();

                    header();
                    Console.WriteLine("¿Cerrar la tienda? (1 : si, 2 : no)");
                    if (Console.ReadLine() == "1")
                    {
                        break;
                    }
                }
            }
        }
    }
}
