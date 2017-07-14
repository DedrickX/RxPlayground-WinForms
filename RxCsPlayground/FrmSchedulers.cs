﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RxCsPlayground
{
    public partial class FrmSchedulers : Form
    {
        private IDisposable _subscription;
        private IDisposable _numbersSubscription;

        public FrmSchedulers()
        {
            InitializeComponent();            
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var clickStream = Observable.FromEventPattern(this.button1, "Click");

            _subscription = clickStream
                .Subscribe(x => { PrintText("ClickStream - New numbers stream");
                                  SubscribeForNumbers(clickStream); },
                           err => PrintText("ClickStream Error!"),
                           () => PrintText("ClickStream Done"));                        
        }
                
        
        protected IObservable<long> GetNumbers()
        {
            return Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }


        protected void SubscribeForNumbers(IObservable<System.Reactive.EventPattern<object>> until)
        {
            _numbersSubscription = GetNumbers()
                .ObserveOn(this)
                .TakeUntil(until)
                .Subscribe(x => PrintText($"#{x}"),
                           err => PrintText("NumbersStream Error!"),
                           () => PrintText("NumbersStream Done"));
        }
                        

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _numbersSubscription?.Dispose();
            _subscription?.Dispose();

        }
        

        /// <summary>
        /// Aktualizuje textbox - pridá nový riadok s oznamom že prebehol click
        /// </summary>
        private void PrintText(string text)
        {
            TxtOtuput.AppendText(text + Environment.NewLine);
        }

    }
}
