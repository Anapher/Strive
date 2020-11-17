let scrollbarWidth: number | undefined = undefined;

export function getScrollbarWidth() {
   if (scrollbarWidth === undefined) {
      // Create the measurement node
      const scrollDiv = document.createElement('div');
      scrollDiv.style.width = '100px';
      scrollDiv.style.height = '100px';
      scrollDiv.style.overflow = 'scroll';
      scrollDiv.style.position = 'absolute';
      scrollDiv.style.top = '-9999px';

      document.body.appendChild(scrollDiv);

      // Get the scrollbar width
      scrollbarWidth = scrollDiv.offsetWidth - scrollDiv.clientWidth;

      // Delete the DIV
      document.body.removeChild(scrollDiv);
   }

   return scrollbarWidth;
}
