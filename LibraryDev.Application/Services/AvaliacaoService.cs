using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Interfaces.Livros;
using LibraryDev.Application.Mappings.Avaliacoes;
using LibraryDev.Application.Queries.Avaliacoes;
using LibraryDev.Application.Validators.Avaliacoes;
using LibraryDev.Application.ViewModels.Avaliacoes;
using LibraryDev.Domain.Interfaces.Avaliacoes;
using LibraryDev.Domain.Interfaces.Livros;
using LibraryDev.Domain.Interfaces.Usuarios;

namespace LibraryDev.Application.Services;

public class AvaliacaoService : IAvaliacaoService
{
    private readonly IAvaliacaoCommandRepository _avaliacaoCommandRepository;
    private readonly IAvaliacaoQueryRepository _avaliacaoQueryRepository;
    private readonly ILivroQueryRepository _livroQueryRepository;
    private readonly ILivroCommandRepository _livroCommandRepository;
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;

    public AvaliacaoService(
        IAvaliacaoCommandRepository commandRepo,
        IAvaliacaoQueryRepository queryRepo,
        ILivroQueryRepository livroQueryRepo,
        ILivroCommandRepository livroCommandRepo,
        IUsuarioQueryRepository usuarioQueryRepo)
    {
        _avaliacaoCommandRepository = commandRepo;
        _avaliacaoQueryRepository = queryRepo;
        _livroQueryRepository = livroQueryRepo;
        _livroCommandRepository = livroCommandRepo;
        _usuarioQueryRepository = usuarioQueryRepo;
    }

    public async Task<IEnumerable<ObterAvaliacoesViewModel>> ObterAvaliacoesAsync()
    {
        var avaliacoes = await _avaliacaoQueryRepository.ObterAvaliacoesAsync();
        return avaliacoes.Select(avaliacao => AvaliacaoMapping.MapToViewModel(avaliacao));
    }

    public async Task<IEnumerable<ObterAvaliacoesViewModel>> ObterAvaliacoesPorLivroAsync(ObterAvaliacoesPorLivroQuery query)
    {
        var avaliacoes = await _avaliacaoQueryRepository.ObterAvaliacoesPorLivroAsync(query.IdLivro);
        return avaliacoes.Select(avaliacao => AvaliacaoMapping.MapToViewModel(avaliacao));
    }

    public async Task<ObterAvaliacaoPorIdViewModel?> ObterAvaliacaoPorIdAsync(ObterAvaliacaoPorIdQuery query)
    {
        var avaliacao = await _avaliacaoQueryRepository.ObterAvaliacaoPorIdAsync(query.Id);
        if (avaliacao is null) return null;

        return new ObterAvaliacaoPorIdViewModel
        (
            avaliacao.Id,
            avaliacao.Nota,
            avaliacao.Descricao,
            avaliacao.IdUsuario,
            avaliacao.Usuario?.Nome ?? string.Empty,
            avaliacao.IdLivro,
            avaliacao.Livro?.Titulo ?? string.Empty,
            avaliacao.DataInicioLeitura,
            avaliacao.DataFimLeitura,
            avaliacao.DataCriacao
        );
    }

    public async Task<(bool sucesso, string mensagem, int id)> CriarAvaliacaoAsync(CriarAvaliacaoCommand command)
    {
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(command);
        if (!valido) return (false, mensagem, 0);

        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(command.IdLivro);
        if (livro is null) return (false, "Livro não encontrado.", 0);

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorIdAsync(command.IdUsuario);
        if (usuario is null) return (false, "Usuário não encontrado.", 0);

        var id = await _avaliacaoCommandRepository.CriarAvaliacaoAsync(CriarAvaliacaoCommand.ToEntity(command));

        // Recalcular e atualizar a nota média do livro
        var novaMedia = await _avaliacaoQueryRepository.CalcularNotaMediaLivroAsync(command.IdLivro);
        await _livroCommandRepository.AtualizarNotaMediaAsync(command.IdLivro, novaMedia);

        return (true, "Avaliação criada com sucesso.", id);
    }

    public async Task<(bool sucesso, string mensagem)> AtualizarAvaliacaoAsync(AtualizarAvaliacaoCommand command)
    {
        var (valido, mensagem) = AvaliacaoValidator.ValidarAtualizar(command);
        if (!valido) return (false, mensagem);

        var avaliacao = await _avaliacaoQueryRepository.ObterAvaliacaoPorIdAsync(command.Id);
        if (avaliacao is null) return (false, "Avaliação não encontrada.");

        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(command.IdLivro);
        if (livro is null) return (false, "Livro não encontrado.");

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorIdAsync(command.IdUsuario);
        if (usuario is null) return (false, "Usuário não encontrado.");

        var resultado = await _avaliacaoCommandRepository.AtualizarAvaliacaoAsync(AvaliacaoMapping.ToEntity(command));
        if (!resultado) return (false, "Não foi possível atualizar a avaliação.");

        // Recalcular nota média após atualização
        var novaMedia = await _avaliacaoQueryRepository.CalcularNotaMediaLivroAsync(command.IdLivro);
        await _livroCommandRepository.AtualizarNotaMediaAsync(command.IdLivro, novaMedia);

        return (true, "Avaliação atualizada com sucesso.");
    }

    public async Task<(bool sucesso, string mensagem)> DeletarAvaliacaoAsync(int id)
    {
        var avaliacao = await _avaliacaoQueryRepository.ObterAvaliacaoPorIdAsync(id);
        if (avaliacao is null) return (false, "Avaliação não encontrada.");

        var idLivro = avaliacao.IdLivro;
        var resultado = await _avaliacaoCommandRepository.DeletarAvaliacaoAsync(id);
        if (!resultado) return (false, "Não foi possível remover a avaliação.");

        // Recalcular nota média após remoção
        var novaMedia = await _avaliacaoQueryRepository.CalcularNotaMediaLivroAsync(idLivro);
        await _livroCommandRepository.AtualizarNotaMediaAsync(idLivro, novaMedia);

        return (true, "Avaliação removida com sucesso.");
    }
}
