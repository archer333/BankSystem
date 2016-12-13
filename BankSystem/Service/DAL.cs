﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Service.Models;

namespace Service
{
    public class DAL
    {
        private static readonly Lazy<DAL> Lazy = new Lazy<DAL>(() => new DAL());
        public static DAL Instance => Lazy.Value;

        private DAL()
        {
            Client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            Database = Client.GetDatabase("banksystem");

            Configurations = Database.GetCollection<ConfigKeyValue>("Configurations");

            if (!Configurations.AsQueryable().Any(config => config.Key == "CurrentAccountId"))
            {
                Configurations.InsertOne(new ConfigKeyValue
                {
                    Key = "CurrentAccountId",
                    Value = "0000000000000000"
                });
            }

            Users = Database.GetCollection<User>("Users");
            Accounts = Database.GetCollection<Account>("Accounts");
            Operations = Database.GetCollection<Operation>("Operations");
                      
        }

        public IMongoClient Client { get; set; }
        public IMongoDatabase Database { get; set; }
        public IMongoCollection<User> Users { get; set; } 
        public IMongoCollection<Account> Accounts { get; set; }
        public IMongoCollection<Operation> Operations { get; set; }
        public IMongoCollection<ConfigKeyValue> Configurations { get; set; }

        //public static IMongoCollection<Artist> Artists;
        //public static IMongoCollection<Song> Songs;
        //public static IMongoCollection<User> Users;
        //public static IMongoCollection<Listen> Listens;
        //public static IMongoCollection<FullListen> FullListens;
        //public static IMongoCollection<Statistics> Statistics;
    }
}
