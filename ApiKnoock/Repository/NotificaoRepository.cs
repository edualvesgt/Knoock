using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.ViewModel;

namespace ApiKnoock.Repository
{
    public class NotificaoRepository : INotificacaoRepository
    {
        KnoockContext _context = new KnoockContext();
        public void Create(NotificacaoViewModel notificacaoViewModel)
        {
            //Isso inicia uma Transaction no banco de dados. Transactions são usadas para garantir a integridade
            //dos dados em operações complexas ou múltiplas.
            using (var transaction = _context.Database.BeginTransaction()) 
            {
                //Neste caso específico, Estamos usando uma transação para garantir que a criação da notificação
                //e (se aplicável) a associação à entrega sejam feitas juntas ou não sejam feitas em absoluto.
                try
                {
                    // Cria uma nova notificação 
                    var notificacao = new Notificacao
                    {
                        TipoUsuarioId = notificacaoViewModel.TipoUsuarioId,
                        Mensagem = notificacaoViewModel.Mensagem,
                        DataNotificacao = notificacaoViewModel.DataNotificacao,
                        Status = notificacaoViewModel.Status,
                        Tipo = notificacaoViewModel.Tipo,
                        ImagemAviso = notificacaoViewModel.ImagemAviso
                    };

                    _context.Notificacaos.Add(notificacao); // Adiciona a notificação ao contexto do banco de dados
                    _context.SaveChanges(); 

                    // Verifica se a notificação é do tipo "entrega" e se um ID de entrega foi fornecido
                    if (notificacaoViewModel.Tipo?.ToLower() == "entrega" && notificacaoViewModel.EntregaId.HasValue)
                    {
                        // Procura a entrega no banco de dados usando o ID fornecido
                        var entrega = _context.Entregas.Find(notificacaoViewModel.EntregaId.Value);

                        if (entrega == null) // Se a entrega não foi encontrada
                        {
                            throw new Exception("A entrega especificada não existe.");
                        }

                        // Cria um registro na tabela intermediária Notificacao_Entrega
                        var notificacaoEntrega = new NotificacaoEntrega
                        {
                            NotificacaoId = notificacao.Id, // Associa a notificação à entrega
                            EntregaId = notificacaoViewModel.EntregaId.Value 
                        };

                        _context.NotificacaoEntregas.Add(notificacaoEntrega); // Adiciona o registro à entidade do contexto
                        _context.SaveChanges(); 
                    }

                    transaction.Commit(); // Confirma a transação, garantindo que todas as operações foram bem-sucedidas
                }
                catch (Exception )
                {
                    throw;

                }
            }
        }


        public List<Notificacao> GetAllNotification()
        {
            try
            {
                return _context.Notificacaos.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

