using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCAP.Api.DataAccess;
using CCAP.Api.Exceptions;
using CCAP.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CCAP.Api.Services {
    public class CreditCardService : ICreditCardService {
        private readonly CCAPContext _context;

        public CreditCardService(CCAPContext context) {
            _context = context;
        }

        public async Task<IList<CreditCardDisplayViewModel>> Get() {
            return await _context
                .CreditCards
                .Select(c => ToViewModel(c))
                .ToListAsync();
        }

        public async Task<CreditCardDisplayViewModel> Get(int id) {
            var viewModel =
                await _context
                .CreditCards
                .Where(c => c.Id == id)
                .Select(c => ToViewModel(c))
                .FirstOrDefaultAsync();

            if (viewModel == null) {
                throw new RecordNotFoundException($"Credit card with id {id} is not found");
            }

            return viewModel;
        }

        private CreditCardDisplayViewModel ToViewModel(CreditCard card) {
            return new CreditCardDisplayViewModel {
                Id = card.Id,
                Category = card.Category,
                SubType = card.SubType,
                CardCode = card.CardCode,
                ImageUrl = card.ImageUrl,
                MinimumCreditScore = card.MinimumCreditScore,
                MinimumLimit = card.MinimumLimit,
                MaximumLimit = card.MaximumLimit,
                MinimumAnnualIncome = card.MinimumAnnualIncome
            };
        }
    }
}