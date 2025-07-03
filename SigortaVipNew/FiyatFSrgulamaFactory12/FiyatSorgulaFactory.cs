using SigortaVip.FiyatSorgulamaFactory.Interface;

namespace SigortaVip.FiyatSorgulamaFactory
{
    public abstract class FiyatSorgulaFactory
    {
        public abstract IFiyatSorgu GetFiyatSorgu(string insuranceCompany);
    }
}
