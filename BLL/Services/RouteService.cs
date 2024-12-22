using System;
using System.Collections.Generic;
using System.Linq;
using DTO = Interfaces.DTO;
using DM = DomainModel;
using Interfaces.Repository;
using Interfaces.Services;
using System.Data.SqlClient;
using System.Data;

namespace BLL.Services
{
    public class RouteService : IRouteService
    {

        public RouteService(IDbRepository repo)
        {
            dbr = repo;
        }

        public bool CanConnect()
        {
            return dbr.CanConnect();
        }
        public List<DTO.RouteInfo> GetAllRouteInfos()
        {
            return dbr.RouteInfos
                    .GetAll()
                    .Select(e => new DTO.RouteInfo()
                    {
                        Id = e.Id,
                        TransportId = e.TransportId,
                        Schedule = e.Schedule
                    })
                    .ToList();
        }

        public DTO.RouteInfo GetRouteInfoById(int id)
        {

            var e = dbr.RouteInfos.Get(id);

            return new DTO.RouteInfo()
            {
                Id = e.Id,
                TransportId = e.TransportId,
                Schedule = e.Schedule
            };

        }

        public void CreateRoute(int routeInfoId, DateTime date)
        {
            var routeInfo = dbr.RouteInfos.Get(routeInfoId);

            if (routeInfo == null)
                throw new Exception("Such route info does not exist");

            if ((routeInfo.Schedule & DateToScheduleMask(date)) == 0)
                throw new Exception("Date does not match schedule");

            var transport = dbr.Transports.Get(routeInfo.TransportId);

            var newRoute = new DM.Route()
            {
                RouteInfoId = routeInfoId,
                LuggageSpace = transport.MaxLuggage,
                Date = date,
                RouteStatusId = 1
            };

            dbr.Routes.Create(newRoute);

            for (int i = 0; i < transport.MaxSeats; ++i)
            {
                dbr.Seats.Create(new DM.Seat()
                {
                    Number = i + 1,
                    RouteId = newRoute.Id,
                    Status = 0,
                    StatusChangedOn = DateTime.Now
                });
            }

            dbr.Save();

        }

        public List<DTO.Destination> GetFullRouteInfo(
            int routeInfoId,
            int? departureId,
            int? arrivalId,
            out decimal adultPrice,
            out decimal underagePrice,
            out int travelHours,
            out byte travelMinutes,
            out TimeSpan departureTime,
            out TimeSpan arrivalTime)
        {
            adultPrice = 0m;
            underagePrice = 0m;
            travelHours = 0;
            travelMinutes = 0;
            departureTime = TimeSpan.Zero;
            arrivalTime = TimeSpan.Zero;

            var routeInfo = dbr.RouteInfos.Get(routeInfoId);

            if (routeInfo == null)
                throw new Exception("Such route info does not exist");

            var rels = dbr.Filters.GetRouteInfoRels(routeInfoId)
                                  .OrderBy(e => e.OrderNumber)
                                  .ToList();

            if (!departureId.HasValue)
            {
                departureId = rels.First().DestinationId;
            }

            if (!arrivalId.HasValue)
            {
                arrivalId = rels.Last().DestinationId;
            }

            int startIndex = rels.IndexOf(rels.Find(r => r.DestinationId == departureId.Value));
            int endIndex = rels.IndexOf(rels.Find(r => r.DestinationId == arrivalId.Value));

            if (startIndex >= endIndex)
                throw new Exception("Departure point is located after arrival point");

            var destinations = new List<DTO.Destination>();

            int prevHour = 0;
            int prevMinute = 0;

            foreach (var rel in rels)
            {

                var d = dbr.Destinations.Get(rel.DestinationId);

                destinations.Add(new DTO.Destination()
                {
                    Id = d.Id,
                    RegionId = d.RegionId,
                    Type = d.Type,
                    TypeInfo = d.TypeInfo,
                    Settlement = d.Settlement,
                    Street = d.Street,
                    House = d.House
                });

                if (rel.OrderNumber >= startIndex && rel.OrderNumber <= endIndex)
                {
                    if (rel.DestinationId > departureId.Value)
                    {
                        adultPrice += rel.AdultPrice;
                        underagePrice += rel.UnderagePrice;
                    }

                    if (rel.DestinationId == departureId.Value)
                        departureTime = new TimeSpan(rel.ArrivalHour, rel.ArrivalMinute, 0);
                    else if (rel.DestinationId == arrivalId.Value)
                        arrivalTime = new TimeSpan(rel.ArrivalHour, rel.ArrivalMinute, 0);

                    if (rel.DestinationId != departureId.Value)
                    {
                        var tsNow = new TimeSpan(rel.ArrivalHour, rel.ArrivalMinute, 0);
                        var tsPrev = new TimeSpan(prevHour, prevMinute, 0);
                        var diff = tsNow - tsPrev;

                        travelMinutes += (byte)diff.Minutes;
                        travelHours += diff.Hours;

                        if (travelMinutes >= 60)
                        {
                            travelHours += travelMinutes / 60;
                            travelMinutes %= 60;
                        }
                    }

                }

                prevHour = rel.ArrivalHour;
                prevMinute = rel.ArrivalMinute;

            }

            return destinations;

        }

        public DTO.RouteInfo GetRouteInfo(int routeId)
        {
            var route = dbr.Routes.Get(routeId);

            if (route == null)
                return null;

            var routeInfo = dbr.RouteInfos.Get(route.RouteInfoId);

            if (routeInfo == null)
                return null;

            return new DTO.RouteInfo()
            {
                Id = routeInfo.Id,
                TransportId = routeInfo.TransportId,
                Schedule = routeInfo.Schedule
            };
        }

        public void CreateRouteInfo(DTO.RouteInfo info, IEnumerable<DTO.Rel_RouteInfo_Destination> rels)
        {

            if (info == null || rels == null)
                throw new ArgumentNullException();

            if (info.TransportId <= 0 || info.Schedule == 0)
                throw new ArgumentException("Incorrect info data");

            if (rels.Count() < 2)
                throw new ArgumentException("There must be at least 2 destinations in a route");

            rels = rels.OrderBy(r => r.OrderNumber);

            int i = 0;
            foreach (var rel in rels)
            {
                if (rel.OrderNumber != i)
                    throw new ArgumentException("Destinations' order numbers mismatch");

                if (i != 0)
                {
                    if (rel.AdultPrice == 0m)
                        throw new Exception($"Rel {i} has adult price set to 0");

                    if (rel.UnderagePrice == 0m)
                        throw new Exception($"Rel {i} has underage price set to 0");
                }

                if (dbr.Destinations.Get(rel.DestinationId) == null)
                    throw new Exception($"Rel {i}'s destination does not exist");

                ++i;
            }

            var newRouteInfo = new DM.RouteInfo()
            {
                TransportId = info.TransportId,
                Schedule = info.Schedule
            };

            dbr.RouteInfos.Create(newRouteInfo);
            Save();
            info.Id = newRouteInfo.Id;

            foreach (var rel in rels)
            {
                rel.RouteInfoId = info.Id;

                var newRel = new DM.Rel_RouteInfo_Destination()
                {
                    RouteInfoId = rel.RouteInfoId,
                    DestinationId = rel.DestinationId,
                    OrderNumber = rel.OrderNumber,
                    AdultPrice = rel.AdultPrice,
                    UnderagePrice = rel.UnderagePrice,
                    ArrivalHour = rel.ArrivalHour,
                    ArrivalMinute = rel.ArrivalMinute,
                    ArrivalPlatform = rel.ArrivalPlatform
                };

                dbr.Rels_RouteInfo_Destination.Create(newRel);
            }

            Save();

        }

        public void DeleteRouteInfo(int id) { /* Пометка? */ }

        public List<DTO.RouteInfo> FindRoutes(int departureId, int arrivalId, DateTime date)
        {

            int scheduleMask = DateToScheduleMask(date);

            if (departureId == 0 || arrivalId == 0 || scheduleMask == 0)
                return new List<DTO.RouteInfo>();

            var infos = dbr.Filters.GetRouteInfosBySchedule(scheduleMask);
            var found = new List<DTO.RouteInfo>();

            foreach (var info in infos)
            {
                var rels = dbr.Filters.GetRouteInfoRels(info.Id);
                var dep = rels.FirstOrDefault(r => r.DestinationId == departureId);
                var arr = rels.FirstOrDefault(r => r.DestinationId == arrivalId);

                // Нужные пункты должны присутствовать в маршруте
                if (dep == null || arr == null)
                    continue;

                // Пункт посадки должен быть раньше пункта прибытия
                if (dep.OrderNumber < arr.OrderNumber)
                {

                    var route = dbr.Filters.GetRouteInfoRoute(info.Id, date);

                    // Должен быть рейс на эту дату
                    if (route == null)
                        continue;

                    var seats = dbr.Filters.GetRouteSeats(route.Id);

                    // Должны быть свободные места
                    if (seats.FirstOrDefault(s => s.Status == 0) == null)
                        continue;

                    found.Add(new DTO.RouteInfo()
                    {
                        Id = info.Id,
                        TransportId = info.TransportId,
                        Schedule = info.Schedule
                    });

                }

            }

            return found;

        }

        public List<DTO.RouteInfo> FindRoutesByCompany(int busCompanyId)
        {
            return dbr.Filters.GetRouteInfosByCompany(busCompanyId)
                      .Select(i => new DTO.RouteInfo()
                      {
                          Id = i.Id,
                          TransportId = i.TransportId,
                          Schedule = i.Schedule
                      })
                      .ToList();
        }

        public List<DTO.Route> GetRouteInfoRoutes(int routeInfoId)
        {
            var routeInfo = dbr.RouteInfos.Get(routeInfoId);

            if (routeInfo == null)
                return new List<DTO.Route>();

            return dbr.Filters
                      .GetRouteInfoRoutes(routeInfoId)
                      .Select(r => new DTO.Route()
                      {
                          Id = r.Id,
                          RouteInfoId = r.RouteInfoId,
                          LuggageSpace = r.LuggageSpace,
                          Date = r.Date,
                          RouteStatusId = r.RouteStatusId
                      })
                      .OrderBy(r => r.Date)
                      .ToList();
        }

        public int DateToScheduleMask(DateTime date)
        {
            var weekday = date.DayOfWeek;

            if (weekday == DayOfWeek.Sunday)
                return 0b01000000;

            return 1 << ((int)weekday - 1);
        }

        public int GetScheduleMask(string schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(schedule))
                return 0;

            return schedule[0];
        }

        public List<string> ScheduleMaskToWeekdays(int mask)
        {

            var weekdays = new List<string>();

            if (mask == 0b01111111)
            {
                weekdays.Add("Ежедневно");
                return weekdays;
            }

            for (int i = 0; i < 7; ++i)
            {
                if ((mask & 1) == 1)
                {

                    switch (i)
                    {
                        case 0:
                            weekdays.Add("Пн");
                            break;

                        case 1:
                            weekdays.Add("Вт");
                            break;

                        case 2:
                            weekdays.Add("Ср");
                            break;

                        case 3:
                            weekdays.Add("Чт");
                            break;

                        case 4:
                            weekdays.Add("Пт");
                            break;

                        case 5:
                            weekdays.Add("Сб");
                            break;

                        case 6:
                            weekdays.Add("Вс");
                            break;

                    }

                }

                mask >>= 1;

            }

            return weekdays;

        }

        public List<DateTime> GetRouteTimes(int routeId)
        {

            var times = new List<DateTime>();

            var route = dbr.Routes.Get(routeId);

            if (route == null)
                return times;

            var rels = dbr.Filters.GetRouteInfoRels(route.RouteInfoId);

            if (rels.Count == 0)
                return times;

            var firstRel = rels.First();

            DateTime current = new DateTime(route.Date.Year,
                                            route.Date.Month,
                                            route.Date.Day,
                                            firstRel.ArrivalHour,
                                            firstRel.ArrivalMinute,
                                            0);

            times.Add(current);

            foreach (var rel in rels)
            {

                if (rel == firstRel)
                    continue;

                current = new DateTime(current.Year,
                                       current.Month,
                                       current.Day,
                                       rel.ArrivalHour,
                                       rel.ArrivalMinute,
                                       0);

                if (rel.ArrivalHour < current.Hour)
                {
                    current = current.AddDays(1);
                }

                times.Add(current);

            }

            return times;

        }

        public void GetRouteStatus(int routeInfoId, DateTime date, out int routeId, out int status)
        {
            //dbr.Execs.GetRouteStatus(routeInfoId, date, out routeId, out status);

            var routeInfo = dbr.RouteInfos.Get(routeInfoId);

            if (routeInfo == null)
                throw new ArgumentException("Incorrect route info id");

            var route = dbr.Filters.GetRouteInfoRoute(routeInfoId, date);

            if (route == null)
            {

                if (date < DateTime.Now)
                    throw new Exception("There was no such trip");

                var newRoute = new DM.Route()
                {
                    RouteInfoId = routeInfo.Id,
                    LuggageSpace = routeInfo.Transport.MaxLuggage,
                    Date = date,
                    RouteStatusId = 1
                };

                dbr.Routes.Create(newRoute);
                route = newRoute;

                int seats = routeInfo.Transport.MaxSeats;
                for (int i = 0; i < seats; ++i)
                {
                    dbr.Seats.Create(new DM.Seat()
                    {
                        Number = i + 1,
                        Route = newRoute,
                        Status = 0,
                        StatusChangedOn = DateTime.Now
                    });
                }

            }

            routeId = route.Id;
            status = route.RouteStatusId;

        }

        public List<int> GetFreeRouteSeats(int routeId)
        {
            return dbr.Filters
                      .GetRouteSeats(routeId)
                      .Where(s => s.Status == 0)
                      .Select(s => s.Number)
                      .ToList();
        }

        public int GetFreeLuggageSpace(int routeId)
        {
            var route = dbr.Routes.Get(routeId);

            if (route == null)
                return 0;

            return route.LuggageSpace;

        }

        public void GetRoutePlatforms(int routeId, int? departureId, int? arrivalId, out int? departurePlatform, out int? arrivalPlatform)
        {
            departurePlatform = null;
            arrivalPlatform = null;

            bool lookForDeparture = departureId.HasValue;
            bool lookForArrival = arrivalId.HasValue;
            bool lookForBoth = lookForDeparture && arrivalId.HasValue;

            var route = dbr.Routes.Get(routeId);

            if (route == null)
                return;

            var rels = dbr.Filters.GetRouteInfoRels(route.RouteInfoId);
            DM.Rel_RouteInfo_Destination dep;
            DM.Rel_RouteInfo_Destination arr;

            if (lookForDeparture)
                dep = rels.FirstOrDefault(r => r.DestinationId == departureId.Value);
            else
                dep = rels.FirstOrDefault();

            if (lookForArrival)
                arr = rels.FirstOrDefault(r => r.DestinationId == arrivalId.Value);
            else
                arr = rels.LastOrDefault();

            if (dep == null)
                throw new Exception("There is no such departure point in this route");

            if (arr == null)
                throw new Exception("There is no such arrival point in this route");

            if (dep.OrderNumber >= arr.OrderNumber)
                throw new Exception("Departure point is located after arrival point");

            departurePlatform = dep.ArrivalPlatform;
            arrivalPlatform = arr.ArrivalPlatform;
        }

        public void GetRouteInfoPlatforms(int routeInfoId, out int? departurePlatform, out int? arrivalPlatform)
        {
            departurePlatform = null;
            arrivalPlatform = null;

            var routeInfo = dbr.RouteInfos.Get(routeInfoId);

            if (routeInfo == null)
                return;

            var rels = dbr.Filters.GetRouteInfoRels(routeInfoId);
            var dep = rels.FirstOrDefault();
            var arr = rels.LastOrDefault();

            departurePlatform = dep.ArrivalPlatform;
            arrivalPlatform = arr.ArrivalPlatform;
        }

        public void GetRouteArrivalDepartureTime(int routeId, int departureId, int arrivalId, out DateTime departureTime, out DateTime arrivalTime)
        {

            var route = dbr.Routes.Get(routeId);

            if (route == null)
                throw new Exception("Route does not exist");

            var rels = dbr.Filters.GetRouteInfoRels(route.RouteInfoId);
            var dep = rels.FirstOrDefault(r => r.DestinationId == departureId);
            var arr = rels.FirstOrDefault(r => r.DestinationId == arrivalId);

            if (dep == null) 
                throw new Exception("Departure point is not in this route");

            if (arr == null)
                throw new Exception("Arrival point is not in this route");

            if (dep.OrderNumber >= arr.OrderNumber)
                throw new Exception("Departure point is located after arrival point");

            var times = GetRouteTimes(routeId);

            departureTime = times[dep.OrderNumber];
            arrivalTime = times[arr.OrderNumber];

        }

        public List<DTO.Destination> GetAllDestinations()
        {
            return dbr.Destinations
                      .GetAll()
                      .Select(e => new DTO.Destination()
                      {
                          Id = e.Id,
                          RegionId = e.RegionId,
                          Type = e.Type,
                          TypeInfo = e.TypeInfo,
                          Settlement = e.Settlement,
                          Street = e.Street,
                          House = e.House
                      })
                      .ToList();
        }
        public DTO.Destination GetDestinationById(int id)
        {

            var e = dbr.Destinations.Get(id);

            return new DTO.Destination()
            {
                Id = e.Id,
                RegionId = e.RegionId,
                Type = e.Type,
                TypeInfo = e.TypeInfo,
                Settlement = e.Settlement,
                Street = e.Street,
                House = e.House
            };
        }
        public void CreateDestination(DTO.Destination d)
        {

            var newDestination = new DM.Destination()
            {
                RegionId = d.RegionId,
                Type = d.Type,
                TypeInfo = d.TypeInfo,
                Settlement = d.Settlement,
                Street = d.Street,
                House = d.House
            };

            dbr.Destinations.Create(newDestination);
            d.Id = newDestination.Id;

            dbr.Save();

        }
        public void DeleteDestination(int id) { /* Пометка? */ }

        public List<DTO.Transport> GetAllTransports()
        {
            return dbr.Transports
                      .GetAll()
                      .Select(e => new DTO.Transport()
                      {
                          Id = e.Id,
                          RegistrationNumber = e.RegistrationNumber,
                          MaxSeats = e.MaxSeats,
                          MaxLuggage = e.MaxLuggage,
                          BusCompanyId = e.BusCompanyId,
                          DriverId = e.DriverId,
                          TransportTypeId = e.TransportTypeId
                      })
                      .ToList();
        }
        public DTO.Transport GetTransportById(int id)
        {
            var e = dbr.Transports.Get(id);

            return new DTO.Transport()
            {
                Id = e.Id,
                RegistrationNumber = e.RegistrationNumber,
                MaxSeats = e.MaxSeats,
                MaxLuggage = e.MaxLuggage,
                BusCompanyId = e.BusCompanyId,
                DriverId = e.DriverId,
                TransportTypeId = e.TransportTypeId
            };
        }
        public void CreateTransport(DTO.Transport t)
        {
            var newTransport = new DM.Transport()
            {
                RegistrationNumber = t.RegistrationNumber,
                MaxSeats = t.MaxSeats,
                MaxLuggage = t.MaxLuggage,
                BusCompanyId = t.BusCompanyId,
                DriverId = t.DriverId,
                TransportTypeId = t.TransportTypeId
            };

            dbr.Transports.Create(newTransport);
            t.Id = newTransport.Id;

            dbr.Save();
        }
        public void DeleteTransport(int id) { /* Пометка? */ }

        public List<DTO.Driver> GetAllDrivers()
        {
            return dbr.Drivers
                      .GetAll()
                      .Select(e => new DTO.Driver()
                      {
                          Id = e.Id,
                          FirstName = e.FirstName,
                          MiddleName = e.MiddleName,
                          LastName = e.LastName
                      })
                      .ToList();
        }
        public DTO.Driver GetDriverById(int id)
        {
            var e = dbr.Drivers.Get(id);

            return new DTO.Driver()
            {
                Id = e.Id,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName
            };
        }
        public void CreateDriver(DTO.Driver d)
        {
            var newDriver = new DM.Driver()
            {
                FirstName = d.FirstName,
                MiddleName = d.MiddleName,
                LastName = d.LastName
            };

            dbr.Drivers.Create(newDriver);
            d.Id = newDriver.Id;

            dbr.Save();

        }
        public void DeleteDriver(int id) { /* Пометка? */ }

        public List<DTO.BusCompany> GetAllBusCompanies()
        {
            return dbr.BusCompanies
                      .GetAll()
                      .Select(e => new DTO.BusCompany()
                      {
                          Id = e.Id,
                          Name = e.Name
                      })
                      .ToList();
        }
        public DTO.BusCompany GetBusCompanyById(int id)
        {
            var e = dbr.BusCompanies.Get(id);

            return new DTO.BusCompany()
            {
                Id = e.Id,
                Name = e.Name
            };
        }
        public void CreateBusCompany(DTO.BusCompany bc)
        {
            var newBusCompany = new DM.BusCompany()
            {
                Name = bc.Name
            };

            dbr.BusCompanies.Create(newBusCompany);
            bc.Id = newBusCompany.Id;

            dbr.Save();
        }
        public void DeleteBusCompany(int id) { /* Пометка? */ }

        public DTO.TransportType GetTransportTypeById(int id)
        {
            var e = dbr.TransportTypes.Get(id);

            return new DTO.TransportType()
            {
                Id = e.Id,
                Name = e.Name
            };
        }

        public DTO.Region GetRegionById(int id)
        {
            var r = dbr.Regions.Get(id);

            if (r == null)
                throw new Exception("Region not found");

            return new DTO.Region()
            {
                Id = r.Id,
                Name = r.Name
            };

        }

        public bool Save()
        {
            if (dbr.Save() > 0) 
                return true;

            return false;
        }

        private IDbRepository dbr;

    }

}