using SD.Infrastructure.EntityBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Data
{
    /// <summary>
    /// 角色信息
    /// </summary>
    public class UserInfo : AggregateRootEntity
    {
        public UserInfo()
        {

        }
        //TODO
        [Required]
        [StringLength(64, ErrorMessage = "{0}长度必须介于{2}和{1}之间", MinimumLength = 64)]
        public string UserId { get; set; }

        public int Gold { get; set; } = 0;
        public int Level { get; set; } = 0;
        public int Exp { get; set; } = 0;
        public int Hp { get; set; } = 0;
        public int Mp { get; set; } = 0;
        public int Power { get; set; } = 0;
        public int Guild { get; set; } = 0;
        public int[] Item { get; set; } = { 0 };
    }
}
