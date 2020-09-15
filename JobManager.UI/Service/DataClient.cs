using JobManager.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JobManager.UI
{
    public class DataClient : Observable, IDataClient
    {
        private string errorMessage;
        private bool isAwaiting;
        private readonly ChannelFactory<IRepository> channelFactory;

        public DataClient()
        {
            channelFactory = new ChannelFactory<IRepository>("dataHttpService");  // nacita z App.config            
            Channel = channelFactory.CreateChannel();
            Server_Static = ((IClientChannel)Channel).RemoteAddress.Uri.Authority;
        }

        public static string Server_Static { get; set; }
        public IRepository Channel { get; }
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }
        public bool IsAwaiting
        {
            get => isAwaiting;
            private set => SetProperty(ref isAwaiting, value);
        }

        public async Task<IEnumerable<IEntity>> GetAllAsync(Type type)
        {
            IEnumerable<IEntity> collection = null;

            try
            {
                ErrorMessage = "";
                IsAwaiting = true;

                collection = await Channel.GetAllAsync(type.AssemblyQualifiedName);

                IsAwaiting = false;
            }
            catch (EndpointNotFoundException ex)
            {
                ErrorMessage = "Nie je mozne sa spojit s databazovym servrom.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }            
            catch (Exception ex)
            {
                ErrorMessage = "Na servry sa vyskytla chyba. Opakujte neskor.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }

            return collection;
        }
        public async Task<IEntity> AddAsync(IEntity entity)
        {
            IEntity addedEntity = null;

            try
            {
                ErrorMessage = string.Empty;
                IsAwaiting = true;

                addedEntity = await Channel.AddAsync(entity);

                IsAwaiting = false;
            }
            catch (EndpointNotFoundException ex)
            {
                ErrorMessage = "Nie je mozne sa spojit s databazovym servrom.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Na servry sa vyskytla chyba. Opakujte neskor.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }

            return addedEntity;

        }
        public async Task<IEntity> UpdateAsync(IEntity entity)
        {
            IEntity updatedEntity = null;

            try
            {
                ErrorMessage = string.Empty;
                IsAwaiting = true;

                updatedEntity = await Channel.UpdateAsync(entity);

                IsAwaiting = false;
            }
            catch (EndpointNotFoundException ex)
            {
                ErrorMessage = "Nie je mozne sa spojit s databazovym servrom.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Na servry sa vyskytla chyba. Opakujte neskor.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }

            return updatedEntity;
        }
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            try
            {
                ErrorMessage = string.Empty;
                IsAwaiting = true;

                _ = await Channel.DeleteAsync(entity);

                IsAwaiting = false;
                return true;
            }
            catch (EndpointNotFoundException ex)
            {
                ErrorMessage = "Nie je mozne sa spojit s databazovym servrom.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Na servry sa vyskytla chyba. Opakujte neskor.";
                IsAwaiting = false;
                Trace.TraceError(ex.Message);
            }

            return false;
        }
               

    }
}
