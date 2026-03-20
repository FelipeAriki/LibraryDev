using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Usuarios;
using LibraryDev.Application.Validators.Usuarios;
using LibraryDev.Application.ViewModels.Usuarios;
using LibraryDev.Domain.Interfaces.Usuarios;

namespace LibraryDev.Application.Services;

public class UsuarioService : IUsuarioService
{

    private readonly IUsuarioCommandRepository _usuarioCommandRepository;
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;

    public UsuarioService(IUsuarioCommandRepository usuarioCommandRepository, IUsuarioQueryRepository usuarioQueryRepository)
    {
        _usuarioCommandRepository = usuarioCommandRepository;
        _usuarioQueryRepository = usuarioQueryRepository;
    }

    public async Task<IEnumerable<ObterUsuariosViewModel>> ObterUsuariosAsync()
    {
        var usuarios = await _usuarioQueryRepository.ObterUsuariosAsync();
        return usuarios.Select(u => new ObterUsuariosViewModel
        (
            u.Id,
            u.Nome,
            u.Email,
            u.Avaliacoes.Count
        ));
    }

    public async Task<ObterUsuarioPorIdViewModel?> ObterUsuarioPorIdAsync(ObterUsuarioPorIdQuery query)
    {
        var usuario = await _usuarioQueryRepository.ObterUsuarioComAvaliacoesPorIdAsync(query.Id);
        if (usuario is null) return null;

        return new ObterUsuarioPorIdViewModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Avaliacoes = usuario.Avaliacoes.Select(a => new AvaliacaoDoUsuarioViewModel
            {
                Id = a.Id,
                Nota = a.Nota,
                Descricao = a.Descricao,
                TituloLivro = a.Livro?.Titulo ?? string.Empty,
                DataInicioLeitura = a.DataInicioLeitura,
                DataFimLeitura = a.DataFimLeitura,
                DataCriacao = a.DataCriacao
            }).ToList()
        };
    }

    public async Task<(bool sucesso, string mensagem, int id)> CriarUsuarioAsync(CriarUsuarioCommand command)
    {
        var (valido, mensagem) = UsuarioValidator.ValidarCriar(command);
        if (!valido) return (false, mensagem, 0);

        var emailExistente = await _usuarioQueryRepository.EmailJaCadastradoAsync(command.Email);
        if (emailExistente)
            return (false, "Já existe um usuário cadastrado com este e-mail.", 0);

        var id = await _usuarioCommandRepository.CriarUsuarioAsync(CriarUsuarioCommand.ToEntity(command));
        return (true, "Usuário criado com sucesso.", id);
    }

    public async Task<(bool sucesso, string mensagem)> AtualizarUsuarioAsync(AtualizarUsuarioCommand command)
    {
        var (valido, mensagem) = UsuarioValidator.ValidarAtualizar(command);
        if (!valido) return (false, mensagem);

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorIdAsync(command.Id);
        if (usuario is null) return (false, "Usuário não encontrado.");

        var emailConflito = await _usuarioQueryRepository.EmailJaCadastradoAsync(command.Email, command.Id);
        if (emailConflito)
            return (false, "Já existe outro usuário cadastrado com este e-mail.");

        var resultado = await _usuarioCommandRepository.AtualizarUsuarioAsync(AtualizarUsuarioCommand.ToEntity(command));
        return resultado
            ? (true, "Usuário atualizado com sucesso.")
            : (false, "Não foi possível atualizar o usuário.");
    }

    public async Task<(bool sucesso, string mensagem)> DeletarUsuarioAsync(int id)
    {
        var usuario = await _usuarioQueryRepository.ObterUsuarioPorIdAsync(id);
        if (usuario is null) return (false, "Usuário não encontrado.");

        var resultado = await _usuarioCommandRepository.DeletarUsuarioAsync(id);
        return resultado
            ? (true, "Usuário removido com sucesso.")
            : (false, "Não foi possível remover o usuário.");
    }
}
