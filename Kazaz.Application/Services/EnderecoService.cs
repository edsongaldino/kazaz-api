using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces.Services;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly ApplicationDbContext _context;

        public EnderecoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EnderecoResponseDto> CriarAsync(EnderecoCreateDto dto)
        {
            var entity = new Endereco
            {
                Id = Guid.NewGuid(),
                Cep = dto.Cep,
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                CidadeId = dto.CidadeId
            };

            _context.Enderecos.Add(entity);
            await _context.SaveChangesAsync();

            return ToResponseDto(entity);
        }

        public async Task<EnderecoResponseDto?> ObterPorIdAsync(Guid id)
        {
            var entity = await _context.Enderecos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            return entity is null ? null : ToResponseDto(entity);
        }

        public async Task<EnderecoResponseDto> AtualizarAsync(EnderecoUpdateDto dto)
        {
            var entity = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == dto.Id);
            if (entity is null)
                throw new KeyNotFoundException("Endereço não encontrado.");

            // aplica mudanças do DTO para a entidade
            entity.Cep = dto.Cep;
            entity.Logradouro = dto.Logradouro;
            entity.Numero = dto.Numero;
            entity.Complemento = dto.Complemento;
            entity.Bairro = dto.Bairro;
            entity.CidadeId = dto.CidadeId;

            await _context.SaveChangesAsync();
            return ToResponseDto(entity);
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            var entity = await _context.Enderecos.FindAsync(id);
            if (entity is null) return false;

            _context.Enderecos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static EnderecoResponseDto ToResponseDto(Endereco e) => new()
        {
            Id = e.Id,
            CidadeId = e.CidadeId,
            Cep = e.Cep,
            Logradouro = e.Logradouro,
            Numero = e.Numero,
            Complemento = e.Complemento,
            Bairro = e.Bairro
        };
    }
}
