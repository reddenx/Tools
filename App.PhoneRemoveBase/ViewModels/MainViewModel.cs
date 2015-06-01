﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.PhoneRemoveBase.Models;

namespace App.PhoneRemoveBase.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private readonly Broadcaster Broadcast;

        public MainViewModel()
        {
            Broadcast = new Broadcaster(37019);
        }

        internal void SetBroadcastingStatus(bool isChecked)
        {
            if (isChecked)
            {
                Broadcast.Start();
            }
            else
            {
                Broadcast.Stop();
            }
        }
    }
}