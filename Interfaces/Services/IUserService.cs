using System;
using System.Collections.Generic;

namespace Interfaces.Services
{

    public enum LoginStatus
    {
        Success,
        UserDoesNotExist,
        IncorrectPassword
    }

    public enum RegisterStatus
    {
        Success,
        UserAlreadyExists
    }

    public enum MakeOrderStatus
    {
        Success,
        NoSeatsAvailable,
        RequiredSeatsOccupied,
        NotEnoughLuggageSpace
    }

    public interface IUserService : IService
    {
        #region Common

        DTO.User CurrentUser { get; }

        bool IsLoggedIn { get; }

        LoginStatus Login(string login, string password);

        void Logout();

        MakeOrderStatus MakeOrder(DTO.Order order, List<DTO.Ticket> tickets);

        #endregion

        #region Passenger

        RegisterStatus Register(DTO.Passenger passenger);

        List<DTO.Order> GetOrders();

        List<DTO.Ticket> GetOrderTickets(int orderId);

        Dictionary<DTO.Order, List<DTO.Ticket>> GetPersonalReport(DateTime? start, DateTime? end);

        #endregion


    }

}