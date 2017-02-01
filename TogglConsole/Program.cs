using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TogglAPI;

namespace TogglConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test().Wait();
        }

        public static async Task Test()
        {
            Authentication au = new Authentication();
            string result = await au.Logon();
            Console.WriteLine(result);

            Console.WriteLine("Program done");
            Console.ReadLine();
        }

        public static async Task CallRecipeServiceAsync()
        {
            var recipes = await new Authentication().GetRecipeDataItemsAsync();
            foreach (var receipe in recipes)
            {
                Console.WriteLine(receipe.Title);
            }
        }
    }
}
