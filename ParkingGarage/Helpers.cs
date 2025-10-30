using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGarage
{
    class Helpers
    {
        static Random rand = new Random();
        public static string RandomReg()
        {
            //String to return
            string reg ="";

            //Alphabet A-z
            string alphabet = "A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z";

            //Remove spaces
            alphabet = alphabet.Replace(" ", "");

            //Divide int o substrings
            string[] letters = alphabet.Split(",");

            //For loop for 3 letters
            for(int i = 0; i<3; i++)
            {
                //Expand this string
                reg += letters[rand.Next(0, letters.Length)];
            }
            //Dash in the middle for clarity sake
            reg += "-";
            //3 Numbers
            for(int i = 0; i<3; i++)
            {
                //0-9
                reg += rand.Next(0, 10);
            }


            return reg;
        }

        public static string RandomColor()
        {
            string[] colors = {
                "Red",
                "Orange",
                "Yellow",
                "Green",
                "Blue",
                "Purple",
                "Pink",
                "Brown",
                "Gray",
                "Black"
            };

            string randoColor = colors[rand.Next(0, colors.Length)];
            return randoColor;
        }

       
        public static string RenterString(Car vec)
        {
            string yesNoElectric = vec.Electric ? "Yes" : "No";
            return $"{vec.GetType().Name} REG: {vec.RegNr} | Color: {vec.VehicleColor} | Electric?: {yesNoElectric}";
        }
        public static string RenterString(Motorcycle vec)
        {
            return $"{vec.GetType().Name} REG: {vec.RegNr} | Color: {vec.VehicleColor} | Make: {vec.BikeMake}";
        }
        public static string RenterString(Bus vec)
        {
            return $"{vec.GetType().Name} REG: {vec.RegNr} | Color: {vec.VehicleColor} | Max Capacity: {vec.MaxPassagerCount}";
        }

    }
}
