using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Interfaces.Livros;
using LibraryDev.Application.Queries.Avaliacoes;
using LibraryDev.Application.ViewModels.Avaliacoes;
using LibraryDev.Domain.Interfaces.Avaliacoes;
using LibraryDev.Domain.Interfaces.Usuarios;

namespace LibraryDev.Application.Services;

public class AvaliacaoService : IAvaliacaoService
{
    private readonly ILivroQueryRepository _livroQueryRepository;
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;
    private readonly IAvaliacaoCommandRepository _avaliacaoCommandRepository;
    private readonly IAvaliacaoQueryRepository _avaliacaoQueryRepository;

    public AvaliacaoService(ILivroQueryRepository livroQueryRepository, IUsuarioQueryRepository usuarioQueryRepository, IAvaliacaoCommandRepository avaliacaoCommandRepository, IAvaliacaoQueryRepository avaliacaoQueryRepository)
    {
        _livroQueryRepository = livroQueryRepository;
        _usuarioQueryRepository = usuarioQueryRepository;
        _avaliacaoCommandRepository = avaliacaoCommandRepository;
        _avaliacaoQueryRepository = avaliacaoQueryRepository;
    }

    public async Task<IEnumerable<ObterAvaliacoesViewModel>> ObterAvaliacoes()
    {
        var avaliacoes = await _avaliacaoQueryRepository.ObterAvaliacoesAsync();
        return avaliacoes.Select(a => new ObterAvaliacoesViewModel(a.Id, a.Nota, a.Descricao, a.IdUsuario, a.IdLivro, a.DataCriacao));
    }

    public async Task<ObterAvalicaoPorIdViewModel> ObterAvalicaoPorId(ObterAvaliacaoPorIdQuery query)
    {
        var avaliacao = await _avaliacaoQueryRepository.ObterAvaliacaoPorIdAsync(query.Id);
        if(avaliacao is null) return null;
        return new ObterAvalicaoPorIdViewModel
        (
            avaliacao.Id,
            avaliacao.Nota,
            avaliacao.Descricao,
            avaliacao.IdUsuario,
            avaliacao.IdLivro,
            avaliacao.DataCriacao
        );
    }

    public async Task<int> CriarAvaliacaoAsync(CriarAvaliacaoCommand command)
    {
        var validacao = await ValidarLivroUsuarioExistente(command.IdLivro, command.IdUsuario);
        if(validacao == 1)
        {
            return await _avaliacaoCommandRepository.CriarAvaliacaoAsync(CriarAvaliacaoCommand.ToEntity(command));
        }
        return 0;
    }

    private async Task<int> ValidarLivroUsuarioExistente(int idLivro, int idUsuario)
    {
        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(idLivro);
        var usuario = await _usuarioQueryRepository.ObterUsuarioPorIdAsync(idUsuario);
        if (livro is null || usuario is null) return 0;
        return 1;
    }

    public async Task<bool> AtualizarAvaliacaoAsync(AtualizarAvaliacaoCommand avaliacao)
    {
       return await _avaliacaoCommandRepository.AtualizarAvaliacaoAsync(AtualizarAvaliacaoCommand.ToEntity(avaliacao));
    }

    public async Task<bool> DeletarAvaliacaoAsync(int id)
    {
        return await _avaliacaoCommandRepository.DeletarAvaliacaoAsync(id);
    }
}
