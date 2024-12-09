using ApiKnoock.Interface;
using ApiKnoock.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoRepository _veiculoRepository;

        public VeiculoController()
        {
            _veiculoRepository = new VeiculoRepository();
        }


    }
}
