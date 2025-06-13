function registerServiceWorker() {
  if (typeof window !== "undefined") {
    if ("serviceWorker" in navigator) {
		
// Sử dụng hàm để lấy thời gian hiện tại định dạng "yyMMddHHmmss"
let currentDateTimeFormatted = getCurrentDateTimeFormatted();
      navigator.serviceWorker.register("/sw.js"+"?d="+currentDateTimeFormatted).then((registration) => {
        // console.log("Service Worker registration successful:", registration);
      });
    }
  }
}

function getCurrentDateTimeFormatted() {
    let now = new Date();
    
    let year = now.getFullYear().toString().substr(-2); // Lấy 2 chữ số cuối của năm
    let month = ('0' + (now.getMonth() + 1)).slice(-2); // Tháng (được cộng thêm 1 vì tháng bắt đầu từ 0)
    let day = ('0' + now.getDate()).slice(-2); // Ngày
    let hours = ('0' + now.getHours()).slice(-2); // Giờ
    let minutes = ('0' + now.getMinutes()).slice(-2); // Phút
    let seconds = ('0' + now.getSeconds()).slice(-2); // Giây
    
    // Định dạng theo "yyMMddHHmmss"
    let formattedDateTime = year + month + day + hours + minutes + seconds;
    
    return formattedDateTime;
}

registerServiceWorker();
