using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Introduction
{
    class Program
    {
        static void Main(string[] args)
        {
            //string strPath = @"C:\windows";
            //ShowLargeFilesWithoutLinq(strPath);
            //Console.WriteLine("*************");
            //ShowLargeFilesWithLinq(strPath);

            //////////////////////////// CSV /////////////////////////////////
            var movies = new List<Movie>
            {
                new Movie { Title = "The Dark Knight", Rating = 6.7f, Year = 2001 },
                new Movie { Title = "The King's speech", Rating = 8.2f, Year = 2002 },
                new Movie { Title = "Casablanca", Rating = 7.6f, Year = 2003 },
                new Movie { Title = "Star Wars 5", Rating = 5.6f, Year = 1998 }
            };

            // own extension method - execution
            var query1 = movies.Filter(m => m.Year > 2000).Take(1);

            Console.WriteLine(query1.Count());

            foreach (var movie in query1)
                Console.WriteLine(movie.Title);

            var enumerator1 = query1.GetEnumerator();
            while (enumerator1.MoveNext())
                Console.WriteLine(enumerator1.Current.Title);

            // other way, the same

            var query2 = movies.Where(m => m.Year > 2000).Take(1);

            // ToList() forces the query to occur -
            // no foreach required to actually access items behind the scene
            var query3 = movies.Where(m => m.Year > 2000).ToList();

            var query4 = Enumerable.Empty<Movie>(); // gets empty collecetion

            var query5 = movies.Where(m => m.Year > 2000) // where jest to streaming operator (classification of standard linq operators)
                               .OrderByDescending(m => m.Rating); // robic dopiero po where, optymalizuje ilosc

            var enumerator2 = query5.GetEnumerator();
            while (enumerator2.MoveNext())
                Console.WriteLine(enumerator2.Current.Title);

            var query6 = from movie in movies
                         where movie.Year > 2000
                         orderby movie.Rating descending
                         select movie;

            var numbers = MyLinq.Random().Where(n => n > 0.5).Take(10);

            foreach (var number in numbers)
                Console.WriteLine(number);

            /////////////////////////////////////////////////////////////////////////
            /// module 5 filter order and project
            /// 

            var cars = ProcessFile1("fuel.csv");

            foreach (var car in cars)
            {
                Console.WriteLine(car.Name);
            }

            var query11 = cars.OrderByDescending(c => 1); // all items
            var query22 = cars.OrderByDescending(c => c.Combined)
                              .ThenByDescending(c => c.Name); // again sort - next critiera

            // alternatively
            var query33 = from car in cars
                          where car.Manufacturer == "BMW" && car.Year == 2013
                          orderby car.Combined descending, car.Name ascending
                          select car;

            foreach (var car in query22.Take(10))
            {
                Console.WriteLine($"{car.Name}:{car.Combined}");
            }

            var query44 = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                              .OrderByDescending(c => c.Combined)
                              .ThenBy(c => c.Name)
                              .Select(c => c);

            foreach (var car in query44.Take(10))
            {
                Console.WriteLine($"{car.Manufacturer}:{car.Name}:{car.Combined}");
            }

            var query55 = from car in cars
                          where car.Manufacturer == "BMW" && car.Year == 2016
                          orderby car.Combined descending, car.Name ascending
                          select car;

            var top1 = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                              .OrderByDescending(c => c.Combined)
                              .ThenBy(c => c.Name)
                              .Select(c => c)
                              .First(); // gets first item, so we cannot iterate

            Console.WriteLine(top1.Name);

            var top2 = cars.OrderByDescending(c => c.Combined)
                              .ThenBy(c => c.Name)
                              .Select(c => c)
                              .FirstOrDefault(c => c.Manufacturer == "BMS" && c.Year == 2016); // worse performance as there is no where
                                                                                               // first may produce exception if conditions provided - put it to where instead, that is better

            // quantifying

            var result222 = cars.Any(c => c.Manufacturer == "Ford"); // is there anything in the dataset - returns bool
            var result333 = cars.All(c => c.Manufacturer == "Ford"); // if all items in dataset - returns bool

            // projecting

            var resultProcessedFileCars = ProcessFile1("fuel.csv");
            var resultPorcessedFileManufacturers = ProcessFileManufacturers("manufacturers.csv");

            var query55555 = from car in resultProcessedFileCars
                             where car.Manufacturer == "BMW" && car.Year == 2016
                             orderby car.Combined descending, car.Name ascending
                             select car;

            foreach (var car in query55555.Take(10))
            {
                Console.WriteLine($"{car.Manufacturer}:{car.Name}:{car.Combined}");
            }

            //var anon = new
            //{
            //    Name = "Scott"
            //};

            //anon.Name;

            var query66666 = from car in resultProcessedFileCars
                             where car.Manufacturer == "BMW" && car.Year == 2016
                             orderby car.Combined descending, car.Name ascending
                             select new
                             {
                                 car.Manufacturer,
                                 car.Name,
                                 car.Combined
                             }; // ograniczamy ilosc kolumn do powyzszej listy (np gdyby bylo tego wiecej

            foreach (var car in query66666.Take(10))
            {
                Console.WriteLine($"{car.Manufacturer}:{car.Name}:{car.Combined}");
            }

            var result77777 = cars.Select(c => new { c.Manufacturer, c.Name, c.Combined });

            // flattening operator (passengers of couple of cars example)

            var result88888 = cars.Select(c => c.Name);

            foreach (var name1 in result88888)
            {
                Console.WriteLine(name1);
            }

            // inner loop iterates through each character
            string name = "Scott";
            IEnumerable<char> characters = "Scott";

            foreach (var name2 in result88888)
            {
                foreach (var character in name2)
                {
                    Console.WriteLine(character);
                }
            }

            // the same result with one foreach using SelectMany (flattening)
            var result99999 = cars.SelectMany(c => c.Name).OrderBy(c => c);
            foreach (var name3 in result88888)
            {
                Console.WriteLine(name3);
            }

            var query1a = from car in resultProcessedFileCars
                          join manu in resultPorcessedFileManufacturers
                                on car.Manufacturer equals manu.Name
                          where car.Manufacturer == "BMW" && car.Year == 2016
                          orderby car.Combined descending, car.Name ascending
                          select new
                          {
                              car.Manufacturer,
                              manu.Headquarters,
                              car.Name,
                              car.Combined
                          }; // ograniczamy ilosc kolumn do powyzszej listy (np gdyby bylo tego wiecej)

            foreach (var car in query1a.Take(10))
            {
                Console.WriteLine($"{car.Headquarters}:{car.Name}:{car.Combined}");
            }

            var query1b = resultProcessedFileCars
                    .Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                    .Join(
                        resultPorcessedFileManufacturers,
                        c => c.Manufacturer,
                        m => m.Name,
                        (c, m) => new
                        {
                            c.Manufacturer,
                            m.Headquarters,
                            c.Name,
                            c.Combined
                        })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

            foreach (var car in query1b.Take(10))
            {
                Console.WriteLine($"{car.Headquarters}:{car.Name}:{car.Combined}");
            }

            // less efficient
            var query1c = resultProcessedFileCars
                    .Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                    .Join(
                        resultPorcessedFileManufacturers,
                        c => c.Manufacturer,
                        m => m.Name,
                        (c, m) => new
                        {
                            Car = c,
                            Manufacturer = m
                        })
                    .OrderByDescending(c => c.Car.Combined)
                    .ThenBy(c => c.Car.Name)
                    .Select(c => new
                    {
                        c.Manufacturer.Headquarters,
                        c.Car.Name,
                        c.Car.Combined
                    }
                    );

            foreach (var car in query1c.Take(10))
            {
                Console.WriteLine($"{car.Headquarters}:{car.Name}:{car.Combined}");
            }

            // composite key
            var query1d = from car in resultProcessedFileCars
                          join manu in resultPorcessedFileManufacturers
                                on new { car.Manufacturer, car.Year }
                                equals new { Manufacturer = manu.Name, manu.Year }
                          where car.Manufacturer == "BMW" && car.Year == 2016
                          orderby car.Combined descending, car.Name ascending
                          select new
                          {
                              car.Manufacturer,
                              manu.Headquarters,
                              car.Name,
                              car.Combined
                          }; // ograniczamy ilosc kolumn do powyzszej listy (np gdyby bylo tego wiecej)

            foreach (var car in query1a.Take(10))
            {
                Console.WriteLine($"{car.Headquarters}:{car.Name}:{car.Combined}");
            }

            var query1e = resultProcessedFileCars
                    .Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                    .Join(
                        resultPorcessedFileManufacturers,
                        c => new { c.Manufacturer, c.Year },
                        m => new { Manufacturer = m.Name, m.Year },
                        (c, m) => new
                        {
                            c.Manufacturer,
                            m.Headquarters,
                            c.Name,
                            c.Combined
                        })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

            foreach (var car in query1b.Take(10))
            {
                Console.WriteLine($"{car.Headquarters}:{car.Name}:{car.Combined}");
            }

            // grouping 

            var query2a = from car in resultProcessedFileCars
                          group car by car.Manufacturer.ToUpper() into manufacturer // into m in order to order
                          orderby manufacturer.Key
                          select manufacturer;

            foreach (var group in query2a)
            {
                Console.WriteLine($"{group.Key} has {group.Count()}");

                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            var query2b = cars.GroupBy(c => c.Manufacturer.ToUpper())
                .OrderBy(g => g.Key);

            foreach (var group in query2b)
            {
                Console.WriteLine($"{group.Key} has {group.Count()}");

                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            // groupjoin for hierarchical data

            var query2c = from manufacturer in resultPorcessedFileManufacturers
                          join car in resultProcessedFileCars on manufacturer.Name equals car.Manufacturer
                          into carGroup
                          orderby manufacturer.Name
                          select new
                          {
                              Manufacturer = manufacturer,
                              Cars = carGroup
                          };

            var query2d = resultPorcessedFileManufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, (m, g) =>
              /*select */  new
                           {
                               Manufacturer = m,
                               Cars = g
                           } /* into result */
            ).OrderBy(m => m.Manufacturer.Name);
            /* group result by result.Manufacturer.Headquarters; */

            foreach (var group in query2c)
            {
                Console.WriteLine($"{group.Manufacturer.Name} has {group.Manufacturer.Headquarters}");

                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            //var query2e = from car in resultProcessedFileCars
            //              group car by car.Manufacturer into carGroup // into m in order to order
            //              select new
            //              {
            //                  Name = carGroup.Key,
            //                  Max = carGroup.Max(), // carGroup.Max(c=> c.Combined),
            //                  Min = carGroup.Min(c=>c.Combined),
            //                  Avg = carGroup.Average(c=>c.Combined)
            //              } into result
            //              orderby result.Max descending
            //              select result;

            //foreach (var result in query2e)
            //{
            //    Console.WriteLine($"{result.Name}");
            //    Console.WriteLine($"\t Max: { result.Max }");
            //    Console.WriteLine($"\t Min: { result.Min }");
            //    Console.WriteLine($"\t Avg: { result.Avg }");
            //}

            // the same as above - using extension method (more efficient, one loop only)

            var query2f = resultProcessedFileCars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                {
                    var results = g.Aggregate(
                            new CarStatistics(),
                            (acc, c) => acc.Accumulate(c),
                            acc => acc.Compute()
                        );
                    return new
                    {
                        Name = g.Key,
                        Avg = results.Average,
                        Min = results.Min,
                        Max = results.Max
                    };
                }
                );

            foreach (var result in query2f)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Max: { result.Max }");
                Console.WriteLine($"\t Min: { result.Min }");
                Console.WriteLine($"\t Avg: { result.Avg }");
            }

            //////////////////////// XML ///////////////////////////
            ///

            CreateXML1();
            QueryXML1();

            /////////////////////// Entity Framework /////////////////////////
            ///

            // 
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());

            InsertData();
            QueryData();
        }

        private static void QueryData()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            var query1a = from car in db.Cars
                          orderby car.Combined descending, car.Name ascending
                          select car;

            foreach (var car in query1a.Take(10))
            {
                Console.WriteLine($"{car.Name}:{car.Combined}");
            }

            /////////////////////////////////////////////////////////////////////

            var query1b = db.Cars.Where(c => c.Manufacturer == "BMW").
                            OrderBy(c => c.Combined).
                            ThenBy(c => c.Name).
                            Take(10);

            foreach (var car in query1b)
            {
                Console.WriteLine($"{car.Name}:{car.Combined}");
            }

            /////////////////////////////////////////////////////////////////////

            Func<int, int> square = x => x * x;
            Func<int, int, int> add1 = (x,y) => x + y;

            var resultTemp1 = add1(3, 5);
            Console.WriteLine(resultTemp1);
            Console.WriteLine(add1);

            Expression<Func<int, int, int>> add2 = (x, y) => x + y;

            var resultTemp2 = add2.Compile()(3, 5);
            Console.WriteLine(resultTemp2);
            Console.WriteLine(add2);

            Func<int, int, int> addI = add2.Compile();
            var resultTemp3 = addI(3, 5);

            Console.WriteLine(resultTemp3);
            Console.WriteLine(addI);

            /////////////////////////////////////////////////////////////////////

            var query1c = db.Cars.Where(c => c.Manufacturer == "BMW").
                            OrderBy(c => c.Combined).
                            ThenBy(c => c.Name).
                            Take(10);

            foreach (var car in query1c)
            {
                Console.WriteLine($"{car.Name}:{car.Combined}");
            }

            /////////////////////////////////////////////////////////////////////

            db.Database.Log = Console.WriteLine;

            var query1d = db.Cars.Where(c => c.Manufacturer == "BMW").
                            OrderBy(c => c.Combined).
                            ThenBy(c => c.Name).
                            // ToList(). // to zmienia IQueryable na IEnumerable
                            Take(10).
                            // Select(c => new { Name = c.Name.ToUpper() }). // { Name = c.Name.Split('')} // nie zadziala, bo linq nie umie translacji na SQL
                            ToList(); // musi byc raczej na koncu (wydajnosc)

            Console.WriteLine(query1d.Count());

            foreach (var car in query1d)
            {
                Console.WriteLine($"{car.Name}:{car.Name}");
            }

            /////////////////////////////////////////////////////////////////////

            var query1e = db.Cars.GroupBy(c => c.Manufacturer)
                .Select(g => new
                {
                    Name = g.Key,
                    Cars = g.OrderByDescending(c=>c.Combined).Take(2)
                });

            foreach (var group in query1e)
            {
                Console.WriteLine($"{group.Name}");
                
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}:{car.Combined}");
                }
            }

            var query1f = from car in db.Cars
                          group car by car.Manufacturer into manufacturer
                          select new
                          {
                              Name = manufacturer.Key,
                              // Cars = manufacturer.OrderByDescending(c=>c.Combined.Take(2))
                              Cars = (from car in manufacturer
                                     orderby car.Combined descending
                                     select car).Take(2)
                          };

            foreach (var group in query1e)
            {
                Console.WriteLine($"{group.Name}");

                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}:{car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessFile1("fuel.csv");
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
            }

            db.SaveChanges();
        }

        private static void QueryXML1()
        {
            var document = XDocument.Load("fuel_linq.xml"); // ReadFrom - not for huge files

            var query1a = from element in document.Element("Cars").Elements("Car") // .Descendants("Car")
                        where element.Attribute("Manufacturer")?.Value == "BWM" // ? if null (covers if not exist)
                        select element.Attribute("Name").Value;

            foreach (var name in query1a)
            {
                Console.WriteLine($"{ name }");
            }
        }

        private static void QueryXML2()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";

            var document = XDocument.Load("fuel_linq.xml"); // ReadFrom - not for huge files

            var query = from element in document.Element(ns + "Cars").Elements(ex + "Car") // .Descendants("Car")
                                                        ?? Enumerable.Empty<XElement>() // if something goes wrong
                        where element.Attribute("Manufacturer")?.Value == "BWM" // ? if null (covers if not exist)
                        select element.Attribute("Name").Value;

            foreach (var name in query)
            {
                Console.WriteLine($"{ name }");
            }
        }

        private static void CreateXML2()
        {
            var recordsXML = ProcessFile1("fuel.csv");

            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";

            var documentXML = new XDocument();

            var carsXML = new XElement(ns + "Cars", 
                            from recordsCarsXML in recordsXML
                            select new XElement(ex + "Car",
                                 new XAttribute("Name", recordsCarsXML.Name),
                                 new XAttribute("Combined", recordsCarsXML.Combined),
                                 new XAttribute("Manufacturer", recordsCarsXML.Manufacturer))
                            );

            carsXML.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));
        }

        private static void CreateXML1()
        {
            var recordsCars = ProcessFile1("fuel.csv");
            var resultManufacturers = ProcessFileManufacturers("manufacturers.csv");

            // we make use of Sytem.Xml.Linq
            // <Cars>
            //      <Car>
            //          <Name>abc</Name>
            //          <Combined>21</Combined>
            //          ...
            //      </Car>
            //      ... more cars
            // </Cars>

            var documentXML = new XDocument();
            var carsXML = new XElement("Cars");

            foreach (var record in recordsCars)
            {
                //var carXML = new XElement("Car");
                //var nameXML = new XElement("Name", record.Name);
                //var combinedXML = new XElement("Combined", record.Combined);

                //carsXML.Add(nameXML);
                //carsXML.Add(combinedXML);
                //carsXML.Add(carXML);

                var carXML = new XElement("Car",
                        new XAttribute("Name", record.Name),
                        new XAttribute("Combined", record.Combined),
                        new XAttribute("Manufacturer", record.Manufacturer)
                    );

                carsXML.Add(carXML);
            }

            documentXML.Add(carsXML);
            documentXML.Save("fuel.xml");

            documentXML = new XDocument();
            carsXML = new XElement("Cars",
                           from recordsCarsXML in recordsCars
                           select new XElement("Car",
                                new XAttribute("Name", recordsCarsXML.Name),
                                new XAttribute("Combined", recordsCarsXML.Combined),
                                new XAttribute("Manufacturer", recordsCarsXML.Manufacturer))
                           );

            documentXML.Add(carsXML);
            documentXML.Save("fuel_linq.xml");
        }

        private static List<Car> ProcessFile1(string path)
        {
            // recznie mapujac
            //File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(line => line.Length > 1)
            //    .Select(line =>
            //    {
            //    });

            // uzywajac extension method, linq funkcyjnie
            //return File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(line => line.Length > 1)
            //    .Select(Car.ParseFromCsv)
            //    .ToList();

            // uzywajac extension method, linq select
            var query = from line in File.ReadAllLines(path).Skip(1)
                   where line.Length > 1
                   select Car.ParseFromCsv(line);

            return query.ToList();
        }

        private static List<Manufacturer> ProcessFileManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });

            return query.ToList();
        }

        private static List<Car> ProcessFile2(string path)
        {
            //var query = File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(l => l.Length > 1)
            //    .Select(l => Car.ParseFromCsv(l));

            var query = File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToCar();

            return query.ToList();
        }

        private static void ShowLargeFilesWithLinq(string strPath)
        {
            var query1 = from file in new DirectoryInfo(strPath).GetFiles()
                        orderby file.Length descending
                        select file;

            foreach (var oFile in query1.Take(5))
            {
                Console.WriteLine($"{oFile.Name, -20} : {oFile.Length,10:N0}");
            }

            var query2 = new DirectoryInfo(strPath).GetFiles().OrderByDescending(f => f.Length).Take(5);

            foreach (var oFile in query2)
            {
                Console.WriteLine($"{oFile.Name,-20} : {oFile.Length,10:N0}");
            }
        }

        //private static bool NameStartsWithS(Employee employee)
        //{
        //    return employee.Name.StartsWith("S");
        //}

        private static void ShowLargeFilesWithoutLinq(string i_strPath)
        {
            DirectoryInfo oDinfo = new DirectoryInfo(i_strPath);
            FileInfo[] arrFileInfo = oDinfo.GetFiles();
            Array.Sort(arrFileInfo, new FileInfoComparer());

            foreach (FileInfo oFile in arrFileInfo)
            {
                Console.WriteLine($"{oFile.Name, -20} : {oFile.Length, 10:N0}");
            }

            for (int i = 0; i < 5; i++)
            {
                FileInfo oFile = arrFileInfo[i];
                Console.WriteLine($"{oFile.Name,-20} : {oFile.Length, 10:N0}");
            }
        }
    }

    public class FileInfoComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            return y.Length.CompareTo(x.Length);
        }
    }

    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
        }

        internal CarStatistics Accumulate(Car c)
        {
            Total += c.Combined;
            Count += 1;

            Max = Math.Max(Max, c.Combined);
            Min = Math.Min(Min, c.Combined);

            return this;
        }

        public CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }

        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source /*, Func<string, Car>*/)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3].Replace(".", ",")),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
