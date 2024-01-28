using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DataLayers
{
    public interface ICommonDAL <T> where T : class
    {
        /// <summary>
        /// Lay du lieu va phan trang dua tren ket qua tim kiem
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang (0 nếu không tiến hành phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng thì trả về toàn bộ dữ liệu)</param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");

        /// <summary>
        /// Đếm số dòng thỏa dữ liệu tìm kiếm
        /// </summary>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng thì trả về toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");

        /// <summary>
        /// Bổ sung, thêm dữ liệu vào database. Hàm trả về ID của dữ liệu 
        /// được bổ sung (nếu trả về < 0 tức là lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);

        /// <summary>
        /// Cập nhật đữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);

        /// <summary>
        /// Xóa dứ liệu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// Lấy bản ghi dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary>
        /// Kiểm tra dữ liệu có mã (id) hiện đang được
        /// sử dụng bởi các dữ liệu khác hay không!?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
