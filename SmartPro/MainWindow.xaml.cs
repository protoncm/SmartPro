using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.Models;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancelTokenSource;
        private GUlViewModel dataSource;
        private object pauseObj = new object();
        private BotStates botState = BotStates.Ready;
        private enum BotStates
        {
            Ready,
            Running,
            Pausing,
            Stopped
        }
        public MainWindow()
        {
            InitializeComponent();
            dataSource = new GUlViewModel();
            DataContext = dataSource;
        }

        private async void start_click(object sender, RoutedEventArgs e)
        {
            string message = await Run();
            richTextBox.AppendText(message + "\n");
        }
        private async Task<string> Run()
        {
            botState = BotStates.Running;
            string innerMessage = string.Empty;
            //disable usage gui when start task
            EnableGUI(false);
            //define cancel token
            cancelTokenSource = new CancellationTokenSource();
            var ctk = cancelTokenSource.Token;
            //temp gui thread
            var prevThr = TaskScheduler.FromCurrentSynchronizationContext();
            Action mainAction = new Action(() => {
                while(true)
                {
                    //pause thread... 
                    lock (pauseObj) { }
                    //Doing something..
                    string callbackMessage = DoWork(ctk);
                    //update gui thread
                    Task.Factory.StartNew(() =>
                    {
                        UpdateGUI(callbackMessage, ctk);
                    }, CancellationToken.None, TaskCreationOptions.AttachedToParent, prevThr);
                }
            });

            //run main task with cancel token
            try
            {
                await Task.Factory.StartNew(mainAction, ctk).ContinueWith((ac) => {
                    EnableGUI(true);
                    innerMessage = "Done.";
                    cancelTokenSource.Dispose();
                    botState = BotStates.Stopped;
                }, ctk, TaskContinuationOptions.None, prevThr);
            }
            catch (TaskCanceledException cexp)
            {
                //cancel
                innerMessage = "Stopped.";
            }
            catch (Exception exp)
            {
                //any exceptions
                innerMessage = "Error: '" + exp.Message + "'";
            }
            finally
            {
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
                EnableGUI(true);
                botState = BotStates.Stopped;
                dataSource.PauseButtonText = "Pause";
            }

            return innerMessage;
        }

        private void EnableGUI(bool isEnable)
        {
            dataSource.IsStartButtonEnaled = isEnable; //start button
            dataSource.IsStopButtonEnaled = !isEnable; //stop button
            dataSource.IsPauseButtonEnaled = !isEnable; //pause / resume button
            dataSource.IsSettingsButtonEnaled = isEnable; //setting button
        }

        public string DoWork(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            //do work
            Thread.Sleep(1000);
            return "Send Key...";
        }

        private void UpdateGUI(string message, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            richTextBox.AppendText(message + "\n");
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if(cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                //release pause object if possible
                if (Monitor.IsEntered(pauseObj))
                {
                    Monitor.Exit(pauseObj);
                }
            }
        }
        private void pauseOrResume_Click(object sender, RoutedEventArgs e)
        {
            if(botState == BotStates.Pausing)
            {
                //resume
                Monitor.Exit(pauseObj);
                botState = BotStates.Running;
                dataSource.PauseButtonText = "Pause";
            }
            else
            {
                //pause
                Monitor.Enter(pauseObj);
                botState = BotStates.Pausing;
                dataSource.PauseButtonText = "Resume";
            }
        }
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }
    }
}
