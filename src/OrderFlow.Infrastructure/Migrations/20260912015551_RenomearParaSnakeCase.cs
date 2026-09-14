using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenomearParaSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemPedido_Pedidos_PedidoId",
                table: "ItemPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemPedido_Produtos_ProdutoId",
                table: "ItemPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Usuarios_UsuarioId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pedidos",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPedido",
                table: "ItemPedido");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "Produtos",
                newName: "produtos");

            migrationBuilder.RenameTable(
                name: "Pedidos",
                newName: "pedidos");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "clientes");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "ItemPedido",
                newName: "item_pedido");

            migrationBuilder.RenameColumn(
                name: "Papel",
                table: "usuarios",
                newName: "papel");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "usuarios",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "usuarios",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "usuarios",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SenhaHash",
                table: "usuarios",
                newName: "senha_hash");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "usuarios",
                newName: "criado_em");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_Email",
                table: "usuarios",
                newName: "ix_usuarios_email");

            migrationBuilder.RenameColumn(
                name: "Preco",
                table: "produtos",
                newName: "preco");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "produtos",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Estoque",
                table: "produtos",
                newName: "estoque");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "produtos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "pedidos",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "pedidos",
                newName: "data");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pedidos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "pedidos",
                newName: "cliente_id");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_ClienteId",
                table: "pedidos",
                newName: "ix_pedidos_cliente_id");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "clientes",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "clientes",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "clientes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "refresh_tokens",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "refresh_tokens",
                newName: "token_hash");

            migrationBuilder.RenameColumn(
                name: "SubstituidoPorTokenHash",
                table: "refresh_tokens",
                newName: "substituido_por_token_hash");

            migrationBuilder.RenameColumn(
                name: "RevogadoEm",
                table: "refresh_tokens",
                newName: "revogado_em");

            migrationBuilder.RenameColumn(
                name: "ExpiraEm",
                table: "refresh_tokens",
                newName: "expira_em");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "refresh_tokens",
                newName: "criado_em");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UsuarioId",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_token_hash");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "item_pedido",
                newName: "quantidade");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "item_pedido",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProdutoId",
                table: "item_pedido",
                newName: "produto_id");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "item_pedido",
                newName: "preco_unitario");

            migrationBuilder.RenameColumn(
                name: "PedidoId",
                table: "item_pedido",
                newName: "pedido_id");

            migrationBuilder.RenameIndex(
                name: "IX_ItemPedido_ProdutoId",
                table: "item_pedido",
                newName: "ix_item_pedido_produto_id");

            migrationBuilder.RenameIndex(
                name: "IX_ItemPedido_PedidoId",
                table: "item_pedido",
                newName: "ix_item_pedido_pedido_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_usuarios",
                table: "usuarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_produtos",
                table: "produtos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pedidos",
                table: "pedidos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_clientes",
                table: "clientes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_item_pedido",
                table: "item_pedido",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_item_pedido_pedidos_pedido_id",
                table: "item_pedido",
                column: "pedido_id",
                principalTable: "pedidos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_item_pedido_produtos_produto_id",
                table: "item_pedido",
                column: "produto_id",
                principalTable: "produtos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_pedidos_clientes_cliente_id",
                table: "pedidos",
                column: "cliente_id",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_usuarios_usuario_id",
                table: "refresh_tokens",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_item_pedido_pedidos_pedido_id",
                table: "item_pedido");

            migrationBuilder.DropForeignKey(
                name: "fk_item_pedido_produtos_produto_id",
                table: "item_pedido");

            migrationBuilder.DropForeignKey(
                name: "fk_pedidos_clientes_cliente_id",
                table: "pedidos");

            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_usuarios_usuario_id",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_usuarios",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "pk_produtos",
                table: "produtos");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pedidos",
                table: "pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "pk_clientes",
                table: "clientes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_item_pedido",
                table: "item_pedido");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "produtos",
                newName: "Produtos");

            migrationBuilder.RenameTable(
                name: "pedidos",
                newName: "Pedidos");

            migrationBuilder.RenameTable(
                name: "clientes",
                newName: "Clientes");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "item_pedido",
                newName: "ItemPedido");

            migrationBuilder.RenameColumn(
                name: "papel",
                table: "Usuarios",
                newName: "Papel");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Usuarios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Usuarios",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Usuarios",
                newName: "Ativo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "senha_hash",
                table: "Usuarios",
                newName: "SenhaHash");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "Usuarios",
                newName: "CriadoEm");

            migrationBuilder.RenameIndex(
                name: "ix_usuarios_email",
                table: "Usuarios",
                newName: "IX_Usuarios_Email");

            migrationBuilder.RenameColumn(
                name: "preco",
                table: "Produtos",
                newName: "Preco");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Produtos",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "estoque",
                table: "Produtos",
                newName: "Estoque");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Produtos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Pedidos",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "Pedidos",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Pedidos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "cliente_id",
                table: "Pedidos",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "ix_pedidos_cliente_id",
                table: "Pedidos",
                newName: "IX_Pedidos_ClienteId");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Clientes",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Clientes",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Clientes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RefreshTokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "RefreshTokens",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "RefreshTokens",
                newName: "TokenHash");

            migrationBuilder.RenameColumn(
                name: "substituido_por_token_hash",
                table: "RefreshTokens",
                newName: "SubstituidoPorTokenHash");

            migrationBuilder.RenameColumn(
                name: "revogado_em",
                table: "RefreshTokens",
                newName: "RevogadoEm");

            migrationBuilder.RenameColumn(
                name: "expira_em",
                table: "RefreshTokens",
                newName: "ExpiraEm");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "RefreshTokens",
                newName: "CriadoEm");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_usuario_id",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_TokenHash");

            migrationBuilder.RenameColumn(
                name: "quantidade",
                table: "ItemPedido",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ItemPedido",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "produto_id",
                table: "ItemPedido",
                newName: "ProdutoId");

            migrationBuilder.RenameColumn(
                name: "preco_unitario",
                table: "ItemPedido",
                newName: "PrecoUnitario");

            migrationBuilder.RenameColumn(
                name: "pedido_id",
                table: "ItemPedido",
                newName: "PedidoId");

            migrationBuilder.RenameIndex(
                name: "ix_item_pedido_produto_id",
                table: "ItemPedido",
                newName: "IX_ItemPedido_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "ix_item_pedido_pedido_id",
                table: "ItemPedido",
                newName: "IX_ItemPedido_PedidoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pedidos",
                table: "Pedidos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPedido",
                table: "ItemPedido",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPedido_Pedidos_PedidoId",
                table: "ItemPedido",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPedido_Produtos_ProdutoId",
                table: "ItemPedido",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Usuarios_UsuarioId",
                table: "RefreshTokens",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
