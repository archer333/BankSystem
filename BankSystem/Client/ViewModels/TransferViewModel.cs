﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class TransferViewModel
    {
        [Required, StringLength(26, ErrorMessage = "Incorrect length", MinimumLength = 26)]
        public string DestinationId { get; set; }  
        [Required, MaxLength(256)]     
        public string Title { get; set; }
        public int AmountMain { get; set; }
        [Range(0, 99)]
        public int AmountReminder { get; set; }

        public decimal DecimalAmount()
        {
            var main = (decimal)AmountMain;
            var reminder = (decimal)AmountReminder;

            return main + reminder / 100;
        }
    }
}
