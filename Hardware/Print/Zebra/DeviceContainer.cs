// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hardware.Print.Zebra
{
    public class DeviceContainer
    {

        private static readonly Lazy<DeviceContainer> _instance = new Lazy<DeviceContainer>(() => new DeviceContainer());
        public static DeviceContainer Instance => _instance.Value;

        private Dictionary<Guid, DeviceEntity> zebraDevices = null;

        private DeviceContainer()
        {
            zebraDevices = new Dictionary<Guid, DeviceEntity>();
        }

        public Guid AddDevice (string _ip, int _port)
        {
            Guid id = Guid.NewGuid();

            var zplDeviceSocket = new DeviceSocketTcp(_ip, _port);

            zebraDevices.Add(id, new DeviceEntity(zplDeviceSocket, id));
            return id;
        }

        public void CheckDeviceStatusOn()
        {
            foreach (KeyValuePair<Guid, DeviceEntity> device in zebraDevices)
            {
                device.Value.CheckDeviceStatusOn();
            }
        }

        public void CheckDeviceStatusOff()
        {
            foreach (KeyValuePair<Guid, DeviceEntity> device in zebraDevices)
            {
                device.Value.CheckDeviceStatusOff();
            }

        }

        public void Send(Guid id, string template, string content)
        {
            DeviceEntity curZebraDevice = null;
            if (zebraDevices.TryGetValue(id, out curZebraDevice) ) {
                curZebraDevice.SendAsync(template, content);
            };
        }

        public void Send(Guid id, string content)
        {
            DeviceEntity curZebraDevice = null;
            if (zebraDevices.TryGetValue(id, out curZebraDevice))
            {
                curZebraDevice.SendAsync(content);
            };
        }

        public void SendFirstOrDefault(string template, string content)
        {
            KeyValuePair<Guid, DeviceEntity> curZebraDevicePair = zebraDevices.FirstOrDefault();
            if (curZebraDevicePair.Value != null) {
                curZebraDevicePair.Value.SendAsync(template, content);
            }
        }
    }
}