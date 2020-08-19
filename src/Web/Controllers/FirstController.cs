using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application;
using EFCore.Models.Models;
using Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kogel.Dapper.Extension;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.MsSql;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FirstController : ControllerBase
    {

        private  ApiDBContent _dbContext=null;
        private readonly IUserService _userService;

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="dbContext"></param>
        public FirstController(ApiDBContent dbContext, UserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }
        /// <summary>
        /// 获取测试数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Get()
        {
            this._dbContext = _dbContext.ToRead();//读写分离
            var list = _dbContext.CmNumberInfo.ToList();
            return new JsonResult(list);
            //return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 获取测试数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DapperGet")]
        public JsonResult DapperGet()
        {
            var conn = new SqlConnection("Data Source=.\\SQL2019; Database=Test; User ID=sa; Password=123456; MultipleActiveResultSets=True");
            var list = conn.QuerySet<CmNumberInfo1>().ToList();
            return new JsonResult(list);
            //return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 获取测试数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRead")]
        [PermissionFilter]
        public JsonResult GetRead()
        {
            return new JsonResult(_userService.GetNumberList());
            //return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 新增
        /// </summary>
        [HttpPost]
        public  void Post(CmNumberInfo cmNumberInfo)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            #region EF插入百万数据
            //this._dbContext = _dbContext.ToWrite();//读写分离
            //List<CmNumberInfo> list = new List<CmNumberInfo>(); 
            //for (int i = 0; i < 2_000_000; i++)
            //{
            //    CmNumberInfo cmNumber = new CmNumberInfo();
            //    cmNumber.NumberId = i;
            //    cmNumber.RuleId = 3;
            //    cmNumber.TypeId = 9;
            //    list.Add(cmNumber);
            //    _dbContext.CmNumberInfo.Add(cmNumber);
            //    _dbContext.SaveChanges();
            //}
            //耗时巨大的代码  
            #endregion

            #region Dapper插入百万数据
            var conn = new SqlConnection("Data Source=.\\SQL2019; Database=Test; User ID=sa; Password=123456; MultipleActiveResultSets=True");
            for (int i = 0; i < 1_000_000; i++)
            {
                int result = conn.CommandSet<CmNumberInfo1>()
                  .Insert(new CmNumberInfo1()
                  {
                     NumberId=i,
                     RuleId=3,
                     TypeId=5
                  });
            }
            #endregion
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("Stopwatch总共花费{0}ms.", ts2.TotalMilliseconds);
            Ok("操作成功，耗时"+ ts2.TotalMilliseconds);
        }

        /// <summary>
        /// 修改
        /// </summary>

        [HttpPut]
        public void Put()
        {

        }

        /// <summary>
        /// 删除
        /// </summary>

        [HttpDelete]
        public void Delete()
        {

        }
    }
}