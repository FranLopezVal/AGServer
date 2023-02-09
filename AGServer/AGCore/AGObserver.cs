using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AGServer.AG
{
    public delegate void ChangeEvent(object newval);

    /// <summary>
    /// Guarda un objeto que sus datos son objservados y notifica un cambio a las
    /// funciones subscritas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableObject<T>
    {
        public T value;
        public string headerOfResponse;
        public AGClient? client; //if null = multicast
        public string firstPartOfData;

        public event ChangeEvent? OnChange;
    }


    /// <summary>
    /// Esta clase controla el cambio de algun Objecto del servidor para enviar
    /// los posibles cambios al cliente
    /// </summary>
    ///
    public class AGObserver
	{
		AGServer _srvHndl;

		int _sleepWatch;


		public AGObserver(ref AGServer srvPointer)
		{
			_srvHndl = srvPointer;
		}
        //Cambia el valor de un ObservableObject
        public void ChangeValue<T>(ref ObservableObject<T> obj, T newValue)
        {
            if(EqualityComparer<T>.Default.Equals(obj.value,newValue))
            {
                obj.value = newValue;

                if(obj.client!=null)
                    AGController.InitAction(obj.headerOfResponse,
                    obj.firstPartOfData + obj.value, obj.client);
                else//Envia mensaje multicast en caso de ser NULL
                    _srvHndl.SendMulticast(obj.headerOfResponse,
                    obj.firstPartOfData + obj.value);
            }
        }
	}
}

