import { UAParser } from 'ua-parser-js';

export function getDeviceName() {
   const fragments = new Array<string | undefined>();

   const parsed = new UAParser(navigator.userAgent);
   const device = parsed.getDevice();

   fragments.push(device.vendor);
   fragments.push(device.model);
   const browser = parsed.getBrowser();
   if (browser.name) {
      fragments.push(browser.name);
      fragments.push(browser.version);
   }

   const os = parsed.getOS();
   if (os.name) {
      fragments.push(`(${os.name}${os.version ? ' ' + os.version : null})`);
   }

   return fragments.filter((x) => x).join(' ');
}
