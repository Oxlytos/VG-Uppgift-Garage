using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGarage
{
    internal class Garage
    {
        //List of vehicles which can be cars, mc's or busses
        static List<IVehicle> vehicles = new List<IVehicle>();
        static List<ParkingSpace> parkingSpaces = new List<ParkingSpace>();

        static Random rand  = new Random();
        public static void Run()
        {

            SetupValues();
            ParkingHouse();
        }

        static void SetupValues()
        {
            for (int i = 0; i < 15; i++)
            {
                parkingSpaces.Add(new ParkingSpace());
            }

            for (int i = 0; i < 5; i++)
            {
                Car c = new Car(Helpers.RandomColor(), true);
                c.RegNr = Helpers.RandomReg();
                vehicles.Add(c);
                SetSpaceForVehicle(parkingSpaces[rand.Next(0,parkingSpaces.Count)], c);
                //parkingSpaces[rand.Next(0, parkingSpaces.Count)].Renter = $"{c.GetType().Name} {c.RegNr.ToString()} {c.Color}";
            }
        }

        static void SetSpaceForVehicle(ParkingSpace space, Car vehicle)
        {
            space.AssignSpace(vehicle);
            //space.Renter = $"{vehicle.GetType().Name} {vehicle.RegNr} {vehicle.VehicleColor} Electric: {(vehicle.Electric ? "Yes" : "No")}";
        }
        static void SetSpaceForVehicle(ParkingSpace space, Motorcycle vehicle)
        {
          //  space.Renter = $"{vehicle.GetType().Name} {vehicle.RegNr} {vehicle.VehicleColor}";
        }
        static void SetSpaceForVehicle(ParkingSpace space, Bus vehicle)
        {
            
            //space.Renter = $"{vehicle.GetType().Name} {vehicle.RegNr} {vehicle.VehicleColor}";
        }
        static void ParkingHouse()
        {
            while (true)
            {
                Console.WriteLine("🚗🚗🏢OSCARS AFFORDABLE PARKING GARAGE🏢🚗🚗\n");
                Console.WriteLine("\"Welcome!\"");
                Console.WriteLine("-Options-");
                Console.WriteLine("1.👉 Check for new arrivals");
                Console.WriteLine("2.👉 Handle parking spaces and renters");
                Console.WriteLine("3.👉 Revenue");
                Console.WriteLine("4.👉 Quit");

                string userInput = Console.ReadLine();
                if(Int32.TryParse(userInput, out int input))
                {
                    switch (input) 
                    {
                        case 1:
                            Console.WriteLine("A new vehicle approaches...");
                            NewVehicle();
                            break;
                        case 2:
                            Console.WriteLine("Current spaces and renters");
                            HandleSpaces();
                            break;
                        case 3:
                            Console.WriteLine("Earnings (h)...");
                            break;
                        case 4:
                            Console.WriteLine("Quitting program");
                            return;
                    }
                }
                else
                {
                    Console.WriteLine("Operation not possible, input misunderstood");
                    Thread.Sleep(1000);
                }
                Console.Clear();



            }
        }

        static void NewVehicle()
        {
            Console.WriteLine("The new vehicle is a....");
            Console.WriteLine("1. Car\n2. Motorcycle\n3. Bussd");

            string input = Console.ReadLine();
            if (Int32.TryParse(input, out int choice))
            {

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Car");
                        Console.Write("Color?: ");
                        string color = Console.ReadLine();
                        if (string.IsNullOrEmpty(color))
                        {
                            color = Helpers.RandomColor();
                        }

                        Console.Write("Electric? Y/N");
                        bool electricYesOrNo = false;
                        ConsoleKeyInfo key = Console.ReadKey(true); // true hides the key from being printed
                        Console.WriteLine();
                        if (key.Key == ConsoleKey.Y)
                        {
                            electricYesOrNo = true;
                            Console.WriteLine("Electric car, got it");
                        }
                        else if (key.Key == ConsoleKey.N)
                        {
                            electricYesOrNo = false;
                            Console.WriteLine("Good ol' diesel");
                        }
                        else
                        {
                        }

                        Car newCar = new Car(color, electricYesOrNo);

                        for(int i = 0; i<parkingSpaces.Count; i++)
                        {
                            if (!parkingSpaces[i].Occupied())
                            {
                                SetSpaceForVehicle(parkingSpaces[i], newCar);
                                break;
                            }
                        }
                        break;


                    case 2:
                        Console.WriteLine("MC");
                        break;
                    case 3:
                        Console.WriteLine("Buss");
                        break;
                }

                Console.ReadLine();
            }
        }
        static void HandleSpaces()
        {
            int avalaibleSpaces = parkingSpaces.Where(t => !t.Occupied()).Count();
            Console.WriteLine($"In total we got {parkingSpaces.Count} that can be rented, {avalaibleSpaces} are available");
            for (int i = 0; i < parkingSpaces.Count; i++)
            {
                parkingSpaces[i].CheckIfOccupied();
                string spaceLeftYesNo = parkingSpaces[i].Occupied() ? "Yes" : "No";
                Console.WriteLine($"{i+1} Occupied : {spaceLeftYesNo} | {parkingSpaces[i].RenterInfo}");
            }
            Console.ReadLine();
        }
    }
    

}
