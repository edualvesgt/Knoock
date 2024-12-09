    using ApiKnoock.Domains;
    using ApiKnoock.ViewModel;

    namespace ApiKnoock.Interface
    {
        public interface ICondominoRepository
        {
            Task Create(CondominoViewModel condominoViewModel, string imageUrl);

            Condomino SearchById(Guid id);

            List<Condomino> GetAllResident();
        }
    }
