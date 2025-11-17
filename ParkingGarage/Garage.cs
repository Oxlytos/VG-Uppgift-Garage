using System;
using System.Collections.Generic;
using System.Drawing;
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

        //Total earned
        static internal decimal totalEarnings = 0;

        //Pricer per s => per m 1.5kr
        static internal decimal pricePerS = 0.025m;
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
            //Create x amount
            for (int i = 0; i < 30; i++)
            {
               // Motorcycle c = new Motorcycle(Helpers.RandomColor(), "Yamaha");
               // c.RegNr = Helpers.RandomReg();
               // vehicles.Add(c);
            }
            ParkInitalVehicles();
        }

        static void ParkInitalVehicles()
        {
            //Handle all the MCs
            var mcs = vehicles.OfType<Motorcycle>().ToList();

            foreach (Motorcycle mc in mcs)
            {
                var possibleSpaceToShare = Helpers.FindViableNeighbouringSpot(parkingSpaces, mc);

                 SetSpaceForVehicle(possibleSpaceToShare[0], mc);
            }
           
        }
        //We give a date when we leave to calc money earned
        static internal void MoreEarnings(DateTime parkDate)
        {
            TimeSpan timeDifference = DateTime.Now - parkDate;
            Console.WriteLine("Time differnce is: " + timeDifference);
            decimal newEarnings = (decimal)timeDifference.TotalSeconds * pricePerS;
            Console.WriteLine("Earned: " + newEarnings);
            CalculateEarnings(newEarnings);
        }
        static internal void CalculateEarnings(decimal moreMoney)
        {
            totalEarnings += moreMoney;
        }


        static void SetSpaceForVehicle(ParkingSpace space, IVehicle vehicle)
        {
            space.AssignSpace(vehicle);
        }
        //The bus needs 2 clear adjecent spaces to park properly
        static void SetSpaceForVehicle(List<ParkingSpace> spaces, Bus vehicle)
        {
            spaces[0].AssignSpace(vehicle);
            spaces[1].AssignSpace(vehicle);
        }
        static void ParkingHouse()
        {
            while (true)
            {
                Console.WriteLine("🚗🚗🏢OSCARS AFFORDABLE PARKING GARAGE🏢🚗🚗\n");
                Console.WriteLine($"Earnings (kr): {totalEarnings.ToString("0.####")} Price per/s: {pricePerS}, per/m {(pricePerS * 60).ToString("0.##")}");
                Console.WriteLine("\"Welcome!\"");
                Console.WriteLine("-Options-");
                Console.WriteLine("1.👉 Check for new arrivals");
                Console.WriteLine("2.👉 Handle parking spaces and renters");
                Console.WriteLine("3.👉 Quit");


                string userInput = Console.ReadLine();
                if (Int32.TryParse(userInput, out int input))
                {
                    switch (input)
                    {
                        case 1:
                            Console.WriteLine("A new vehicle approaches...");
                            NewVehicle();
                            break;
                        case 2:
                            Console.WriteLine("Current spaces and renters");
                            ParkingSpacesList();
                            break;
                        case 3:
                            Console.WriteLine("Quitting program");
                            return;
                        default:
                            Console.WriteLine("Please try again");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Operation not possible, input misunderstood");
                    Console.ReadLine();    
                }
                Console.Clear();
            }
        }

        static void NewVehicle()
        {
            Console.WriteLine("The new vehicle is a....");
            Console.WriteLine("1. Car\n2. Motorcycle\n3. Bus");

            string input = Console.ReadLine();
            if (!Int32.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input!");
                return;
            }
            IVehicle vec = null;

            switch (choice)
            {
                //Car
                case 1:
                    if (!Helpers.CheckIfWeCanFitCar(parkingSpaces))
                    {
                        Helpers.InformUserOfParkingError();
                        return;
                    }
                    vec = CreateCar();
                    break;
                //MC
                case 2:
                    if (!Helpers.CheckIfWeCanFitMC(parkingSpaces))
                    {
                        Helpers.InformUserOfParkingError();
                        return;
                    }
                    vec = CreateMotorcycle();
                     break;
                //Bus
                case 3:
                    if(!Helpers.CheckIfBusCanFit(parkingSpaces))
                    {
                        Helpers.InformUserOfParkingError();
                        return;
                    }
                    vec = CreateBus();
                    break;
                default:
                    Console.WriteLine("Error, invalid choice");
                    Console.ReadLine();
                    break;
            }

            Console.ReadLine();
        }

        static Car CreateCar()
        {
            //Helper question
            string color = Helpers.AskColour();
            Console.Write("Electric? Y/N");
            bool electricYesOrNo = false;
            ConsoleKeyInfo key = Console.ReadKey(true);
            Console.WriteLine();
            //ask, handle input, default to diesel
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
                electricYesOrNo = false;
                Console.WriteLine("Defaulting to diesel");
            }
            //Create this car
            Car newCar = new Car(color, electricYesOrNo);

            //Find space
            List<ParkingSpace> carSpace = Helpers.FindViableNeighbouringSpot(parkingSpaces, newCar);

            SetSpaceForVehicle(carSpace[0], newCar);
            return newCar;

        }

        static Motorcycle CreateMotorcycle()
        {
            string mColor = Helpers.AskColour();
            Console.WriteLine("What make is this bike?");
            string make = Console.ReadLine();
            //If empty, just log it as Undefined
            if (string.IsNullOrEmpty(make))
            {
                make = "Undefined";
            }
            Motorcycle newBike = new Motorcycle(mColor, make);
            List<ParkingSpace> spots = Helpers.FindViableNeighbouringSpot(parkingSpaces, newBike);

            SetSpaceForVehicle(spots[0], newBike);
            return newBike;

        }

       
        static Bus CreateBus()
        {
            string bussColour = Helpers.AskColour();
            Console.WriteLine("How many people can the bus fit?");
            string userCapcacityInput = Console.ReadLine();
            int baseBusCap = 20;
            //Some basevalue for a standard capacity bus, lets say we'll default to 20
            if (string.IsNullOrEmpty(userCapcacityInput))
            {
                Console.WriteLine("Registrering this at standard capacity of 20 people");
            }
            //If they've inputed an int well register it, else it will be just 20
            else if (Int32.TryParse(userCapcacityInput, out int busCapacity))
            {
                baseBusCap = busCapacity;
            }


            Bus newBus = new Bus(bussColour, baseBusCap);
            List<ParkingSpace> busSpots = Helpers.FindViableNeighbouringSpot(parkingSpaces,newBus);
            //Console.WriteLine("Found this many spots: " +  busSpots.Count);
            SetSpaceForVehicle(busSpots, newBus);
            return newBus;
           
        }

        static void ParkingSpacesList()
        {
            while (true)
            {
                Console.Clear();
                int avalaibleSpaces = parkingSpaces.Where(t => !t.Occupied()).Count();
                Console.WriteLine($"In total we got {parkingSpaces.Count} that can be rented, {avalaibleSpaces} are available");
                for (int i = 0; i < parkingSpaces.Count; i++)
                {
                    parkingSpaces[i].CheckIfOccupied();
                    string spaceLeftYesNo = parkingSpaces[i].Occupied() ? "Yes" : "No";
                    Console.WriteLine($"{i + 1} Occupied : {spaceLeftYesNo}  {parkingSpaces[i].RenterInfo}");
                    for (int j = 0; j < 60; j++)
                    {
                        Console.Write("-");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("====Operations======");
                Console.WriteLine("1. Checkout a vehicle");
                Console.WriteLine("2. Return to main menu");
                string userInput = Console.ReadLine();


                if (!Int32.TryParse(userInput, out int userMenuChoice))
                {
                    Console.WriteLine("Please input a number");
                    Console.ReadKey();
                    continue;
                }
                switch (userMenuChoice)
                {
                    case 1:
                        Console.WriteLine("Please input the REG of the vehicle you wish to checkout");
                        string userRegInput = Console.ReadLine();
                        if (string.IsNullOrEmpty(userRegInput))
                        {
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            bool succes = CheckoutVehicle(userRegInput.ToUpper());
                            if (succes)
                            {
                                Console.WriteLine("Vehicle removed!");
                            }
                            else
                            {
                                Console.WriteLine("Error somewhere, couldn't be removed...");
                            }
                            Console.ReadLine();
                            break;
                        }
                    case 2:
                        Console.WriteLine("Returning to main menu...");
                        Console.ReadKey();
                        return;
                    default:
                        Console.WriteLine("Invalid option or operation, please try again");
                        Console.ReadKey();
                        break;
                }
            }

        }
        static bool CheckoutVehicle(string reg)
        {
            //Find spaces with vehicles with matching plates/REGs
            var parkingSpacesWithReg = parkingSpaces.Where(s => s.OccupyingVehicles.Keys.Any(v => v.RegNr.Equals(reg))).ToList();

            bool succesBool = false;

            //If we find a vehicle/s with this REG
            //I E make sure a bus that occupies 2 spots gets removed
            if (parkingSpacesWithReg.Any())
            {
                foreach (var s in parkingSpacesWithReg)
                {
                    //Make em' leave
                    succesBool = s.VehicleExuent(reg);
                }
            }
            return succesBool;
        }

    }


}
