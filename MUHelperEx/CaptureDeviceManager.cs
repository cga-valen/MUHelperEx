using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;

namespace MUHelperEx {
    public class CaptureDeviceManager {

        private static CaptureDeviceList cdl = null;

        private CaptureDeviceManager() {

        }

        public static CaptureDeviceList getInstance() {
            if (cdl == null) {
                //cdl = CaptureDeviceList.Instance;
                cdl = CaptureDeviceList.New();
            }
            return cdl;
        }

    }
}
