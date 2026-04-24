import Swal from 'sweetalert2';
import { ApiError } from '../model/base/ApiError.model';

export class ApiErrorHandler {
  /**
   * Hàm xử lý và hiển thị thông báo lỗi API bằng SweetAlert2.
   * Gọi hàm này bên trong block catch hoặc error của subscribe.
   * 
   * @param err Đối tượng lỗi nhận được từ API
   * @param defaultTitle Tiêu đề thông báo mặc định (nếu không phải lỗi Validation)
   */
  static handleError(err: any, defaultTitle: string = 'Đã có lỗi xảy ra'): void {
    
    // Nếu là ApiError đã được base-api.service bắt và format lại
    if (err instanceof ApiError || err.name === 'ApiError') {
      
      // Trường hợp 1: Có chứa "Errors" nội bộ (ví dụ lỗi 400 Validation báo thiếu tên, sai pass...)
      if (err.errors && Object.keys(err.errors).length > 0) {
        let errorMessage = '<ul style="text-align: left; list-style-type: disc; margin-left: 20px;">';
        
        Object.keys(err.errors).forEach(key => {
          const listLoi = err.errors![key];
          if (Array.isArray(listLoi)) {
            listLoi.forEach(msg => {
              // Ghép tên trường và thông báo (ex: "LastName: Tối thiểu 3 ký tự")
              errorMessage += `<li><b>${key}</b>: ${msg}</li>`;
            });
          } else {
            errorMessage += `<li><b>${key}</b>: ${listLoi}</li>`;
          }
        });
        
        errorMessage += '</ul>';

        Swal.fire({
          icon: 'warning',
          title: err.message || 'Lỗi thông tin', // Sử dụng message gốc làm tiêu đề hoặc mặc định
          html: errorMessage,
          confirmButtonColor: '#1976d2'
        });
      } 
      // Trường hợp 2: Có lỗi nhưng chỉ là thông điệp chuỗi đơn giản
      else {
        Swal.fire({
          icon: 'error',
          title: defaultTitle,
          text: err.message || 'Lỗi từ Backend.',
          confirmButtonColor: '#1976d2'
        });
      }
    } 
    // Các lỗi http thuần (ví dụ 500 server crash) mà base-api.service chưa ném thành ApiError
    else if (err.status === 500) {
      Swal.fire({
        icon: 'error',
        title: 'Lỗi hệ thống',
        text: 'Server đang gặp sự cố. Vui lòng thử lại sau.',
        confirmButtonColor: '#1976d2'
      });
    } 
    // Lỗi rớt mạng hoặc lỗi không xác định
    else {
      Swal.fire({
        icon: 'error',
        title: 'Lỗi kết nối',
        text: err?.error?.message || err?.message || 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra Internet.',
        confirmButtonColor: '#1976d2'
      });
    }
  }
}
