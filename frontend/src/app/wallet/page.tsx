// pages/wallets.tsx
"use client";

import { useRef, useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import { FaPowerOff, FaTrash, FaUserCircle } from "react-icons/fa";
import Wallet from "../models/wallet";

// Definindo a interface para o tipo Wallet

export default function WalletsPage() {
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const [newWalletName, setNewWalletName] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [token] = useState<string | null>(localStorage.getItem("token"));
  const [email] = useState<string | null>(localStorage.getItem("userEmail"));
  const router = useRouter();
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef(null);

  // Função para buscar as carteiras
  const fetchWallets = async () => {
    try {
      setLoading(true);
      const response = await fetch("http://localhost:8080/api/wallet/user", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
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
  }, []);

  const handleViewTransactions = (walletId: string) => {
    router.push(`/transaction?walletId=${walletId}`);
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("userEmail");
    router.push(`/login`);
  };

  // Criar nova carteira
  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newWalletName.trim()) return;

    try {
      const response = await fetch("http://localhost:8080/api/wallet", {
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
      const response = await fetch(
        `http://localhost:8080/api/wallet?id=${id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      if (!response.ok) throw new Error("Erro na requisição");
      fetchWallets();
    } catch {
      setError("Erro ao excluir carteira");
    }
  };

  return (
    <div
      className="p-6 max-w-4xl mx-auto bg-gray-50 min-h-screen relative"
      ref={dropdownRef}
    >
      {/* Barra superior com usuário logado */}
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Minhas Carteiras</h1>

        {/* Usuário Logado */}
        <button
          onClick={() => setShowDropdown((prev) => !prev)}
          className="flex items-center gap-2 text-gray-700 hover:text-black focus:outline-none"
        >
          <FaUserCircle size={24} />
          <span className="font-semibold">{email}</span>
        </button>
        {showDropdown && (
          <div
            className="absolute right-0 w-40 bg-white border rounded shadow-lg z-20"
            style={{ marginTop: "70px" }}
          >
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 px-4 py-2 text-sm text-red-600 hover:bg-red-100 w-full text-left"
            >
              <FaPowerOff />
              Sair
            </button>
          </div>
        )}
      </div>

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
