// pages/wallets.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import { FaTrash } from "react-icons/fa";

// Definindo a interface para o tipo Wallet
interface Wallet {
  id: string;
  name: string;
  balance: number;
}

export default function WalletsPage() {
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const [newWalletName, setNewWalletName] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const router = useRouter();

  // Função para buscar as carteiras
  const fetchWallets = async () => {
    try {
      setLoading(true);
      const response = await fetch("https://localhost:44376/api/wallet");
      if (!response.ok) throw new Error("Erro na requisição");
      const data = await response.json();
      setWallets(data.data);
    } catch {
      setError("Erro ao carregar as carteiras");
    } finally {
      setLoading(false);
    }
  };

  // Carregar carteiras ao montar o componente
  useEffect(() => {
    fetchWallets();

    if (typeof window !== "undefined") {
      setToken(localStorage.getItem("token"));
    }
  }, []);

  const handleViewTransactions = (walletId: string) => {
    router.push(`/transaction?walletId=${walletId}`);
  };

  // Criar nova carteira
  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newWalletName.trim()) return;

    try {
      const response = await fetch("https://localhost:44376/api/wallet", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          name: newWalletName,
          balance: 0,
        }),
      });
      if (!response.ok) throw new Error("Erro na requisição");
      fetchWallets();
      setNewWalletName("");
    } catch {
      setError("Erro ao criar carteira");
    }
  };

  // Excluir carteira
  const handleDelete = async (id: string) => {
    if (!confirm("Tem certeza que deseja excluir esta carteira?")) return;

    try {
      const response = await fetch(`https://localhost:44376/api/wallet/${id}`, {
        method: "DELETE",
      });
      if (!response.ok) throw new Error("Erro na requisição");
      fetchWallets();
    } catch {
      setError("Erro ao excluir carteira");
    }
  };

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Minhas Carteiras</h1>

      {/* Formulário de criação */}
      <form onSubmit={handleCreate} className="mb-6">
        <div className="flex gap-2">
          <input
            type="text"
            value={newWalletName}
            onChange={(e) => setNewWalletName(e.target.value)}
            placeholder="Nome da nova carteira"
            className="border p-2 rounded flex-1"
          />
          <button
            type="submit"
            className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
          >
            Adicionar
          </button>
        </div>
      </form>

      {/* Mensagem de erro */}
      {error && <div className="text-red-500 mb-4">{error}</div>}

      {/* Loading */}
      {loading ? (
        // Exibe skeletons enquanto carrega
        Array(3) // Número de skeletons (ajuste conforme necessário)
          .fill(0)
          .map((_, index) => (
            <div
              key={index}
              className="border p-4 rounded flex justify-between items-center"
            >
              <div className="flex-1">
                <Skeleton height={20} width="60%" />
                <Skeleton height={16} width="40%" />
              </div>
              <div className="flex gap-2">
                <Skeleton height={40} width={80} />
                <Skeleton height={40} width={80} />
              </div>
            </div>
          ))
      ) : (
        <div className="grid gap-4">
          {wallets &&
            wallets.map((wallet) => (
              <div
                key={wallet.id}
                className="border p-4 rounded flex justify-between items-center"
              >
                <>
                  <div>
                    <h2 className="font-semibold">{wallet.name}</h2>
                    <p className="text-gray-600">
                      Saldo: R$ {wallet.balance.toFixed(2)}
                    </p>
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleViewTransactions(wallet.id)}
                      className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-yellow-600"
                    >
                      Visualizar Transações
                    </button>
                    <button
                      onClick={() => handleDelete(wallet.id)}
                      className="ml-4 bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600 flex items-center gap-2"
                    >
                      <FaTrash />
                      Excluir
                    </button>
                  </div>
                </>
              </div>
            ))}
        </div>
      )}
    </div>
  );
}
