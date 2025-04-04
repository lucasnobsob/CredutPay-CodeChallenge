﻿using CredutPay.Domain.Interfaces;
using CredutPay.Domain.Models;
using CredutPay.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CredutPay.Infra.Data.Repository
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public async Task<IEnumerable<Wallet>> GetAllByUserId(Guid Id)
        {
            return await DbSet.AsNoTracking()
                .Include(w => w.User)
                .Where(w => w.UserId == Id.ToString())
                .ToListAsync();
        }
    }
}
