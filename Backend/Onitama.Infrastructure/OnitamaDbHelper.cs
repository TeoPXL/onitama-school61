using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onitama.Infrastructure
{
    public class OnitamaDbHelper
    {
        private OnitamaDbContext _dbContext;
        public OnitamaDbHelper(OnitamaDbContext dbContext) {
            _dbContext = dbContext;
        }

        public void SaveDb()
        {
            _dbContext.SaveChanges();
        }
    }
}
