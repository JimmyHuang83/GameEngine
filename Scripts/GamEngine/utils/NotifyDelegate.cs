using System.Collections.Generic;
using UnityEngine;
namespace GameEngine
{

	public abstract class NotifyParams
	{
		public object value;
		public T getValue<T>()
		{
			return (T)value;
		}
	}
    /// <summary>
    /// 消息处理器
    /// </summary>
    public class NotifyDelegate
    {
        /// <summary>
        /// 消息处理的代理
        /// </summary>
        /// <param name="notifyBody">消息体，内部可以放各种类型的值</param>
		public delegate void NotifyHandleDelegate(NotifyParams notifyBody);


        /// <summary>
        /// 存放消息处理的字典，KEY为消息名，VALUE为处理该消息的代理
        /// </summary>
        private static Dictionary<string, NotifyHandleDelegate> dicNotify = new Dictionary<string, NotifyHandleDelegate>();



        /// <summary>
        /// 注册消息的处理器。每个消息可以注册多个处理器，顺序由注册的顺序决定。同一消息名的同一个处理器注册多次的话，只有最后一个生效
        /// </summary>
        /// <param name="notifyName">消息名</param>
        /// <param name="notifyHandle">消息处理器</param>
        public static void RegisterNotifyHandle(string notifyName, NotifyHandleDelegate notifyHandle)
        {
            // get list of handle by name
            NotifyHandleDelegate theNotifyHandle = null;
            bool isExist = dicNotify.TryGetValue(notifyName, out theNotifyHandle);

            if ((!isExist) || (theNotifyHandle == null))
            {
                theNotifyHandle += notifyHandle;
                // save to dict
                dicNotify[notifyName] = theNotifyHandle;
            }
            else
            {
                // assure one handle add only once, if it add yet, delete it first
                theNotifyHandle -= notifyHandle;

                // add it again, if it add yet, it is deleted yet, so this add action is "only once"
                theNotifyHandle += notifyHandle;

                // save to dict
                dicNotify[notifyName] = theNotifyHandle;
            }
        }

        /// <summary>
        /// 注销消息的处理器。
        /// </summary>
        /// <param name="notifyName">消息名</param>
        /// <param name="notifyHandle">消息处理器</param>
        public static void RemoveNotifyHandle(string notifyName, NotifyHandleDelegate notifyHandle)
        {
            // get list of handle by name
            NotifyHandleDelegate theNotifyHandle = null;
            bool isExist = dicNotify.TryGetValue(notifyName, out theNotifyHandle);

            if ((isExist) && (theNotifyHandle != null))
            {
                // remove it
                theNotifyHandle -= notifyHandle;

                // save to dict
                dicNotify[notifyName] = theNotifyHandle;
            }
        }

        /// <summary>
        /// 广播消息。未被注册的消息，将被丢弃
        /// </summary>
        /// <param name="notifyName">消息名</param>
        /// <param name="notifyBody">消息体</param>
        public static void SendNotification(string notifyName, NotifyParams notifyBody)
        {
            // get list of handle by name
            NotifyHandleDelegate theNotifyHandle = null;
            bool isExist = dicNotify.TryGetValue(notifyName, out theNotifyHandle);

            // execute all the handles
            if (isExist && (theNotifyHandle != null))
            {
				theNotifyHandle(notifyBody);
            }
            else 
            {
                Debug.LogError("--------- Notification handle not found: " + notifyName);
            }
                
        }

		public static void SendNotification(string notifyName)
		{
			// get list of handle by name
			NotifyHandleDelegate theNotifyHandle = null;
			bool isExist = dicNotify.TryGetValue(notifyName, out theNotifyHandle);
			
			// execute all the handles
			if (isExist && theNotifyHandle != null)
			{
				theNotifyHandle(null);
			}
			else 
			{
				Debug.LogError("--------- Notification handle not found: " + notifyName);
			}
		}

    }
}
