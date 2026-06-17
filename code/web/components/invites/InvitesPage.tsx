'use client';

import React, { useEffect, useState, useCallback } from 'react';
import { Loader2, Check, X, Mail, Calendar, Send, UserCheck, Clock, Users, Shield } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { useUser } from '@/providers/UserProvider';
import coWorkerService, { CoWorkerInviteOutputModel, GetMyCoWorkerInfoResponse, GetMyCoWorkerWithEmailInfoResponse } from '@/services/api/CoWorkerService';

export default function TeamManagementPage() {
  const { user, refreshUser } = useUser();
  const isOwner = user?.role === 'creator' || user?.role === 'company';
  const isCoWorker = user?.role === 'coworker';
  const [teamMembers, setTeamMembers] = useState<GetMyCoWorkerWithEmailInfoResponse[]>([]);


  const [receivedInvites, setReceivedInvites] = useState<CoWorkerInviteOutputModel[]>([]);
  const [sentInvites, setSentInvites] = useState<CoWorkerInviteOutputModel[]>([]);

  const [pageLoading, setPageLoading] = useState(true);
  const [actionLoadingId, setActionLoadingId] = useState<number | null>(null);
  const [sendLoading, setSendLoading] = useState(false);

  const [emailInput, setEmailInput] = useState('');

  const activeCoWorkers = sentInvites.filter(
    inv => inv.status.toLowerCase() === 'accepted'
  );

  async function handleRemoveCoWorker(coWorkerUserId: number) {
    if (!confirm("Are you sure you want to remove this connection?")) return;

    let success = false;
    if (isOwner) {
      success = await coWorkerService.ownerCancelCoworking(coWorkerUserId);
    } else {
      success = await coWorkerService.cancelCoworking();
    }

    if (success) {
      await refreshUser();
      loadPageData();
    } else {
      alert("Failed to remove connection.");
    }
  }

  const loadPageData = useCallback(async () => {
    try {
      setPageLoading(true);
      if (isOwner || isCoWorker) {
        const sent = await coWorkerService.getSentInvites();
        setSentInvites(sent);
        const team = await coWorkerService.getMyTeam();
        setTeamMembers(team);
      } else if (user?.role === 'justUser') {
        const received = await coWorkerService.getReceivedInvites();
        setReceivedInvites(received.filter(inv => inv.status === 'WAITING'));
      }
    } catch (error) {
      console.error("Error loading team/invite data:", error);
    } finally {
      setPageLoading(false);
    }
  }, [isOwner, isCoWorker, user?.role]);

  useEffect(() => {
    loadPageData();
  }, [loadPageData]);

  async function handleAccept(inviteId: number) {
    setActionLoadingId(inviteId);
    const success = await coWorkerService.acceptInvite(inviteId);

    if (success) {
      setReceivedInvites(prev => prev.filter(inv => inv.id !== inviteId));

      await refreshUser();

      alert("Invitation accepted! Your status has been updated.");
    } else {
      alert("Failed to accept the invite.");
    }
    setActionLoadingId(null);
  }

  async function handleReject(inviteId: number) {
    setActionLoadingId(inviteId);
    const success = await coWorkerService.rejectInvite(inviteId);
    if (success) {
      setReceivedInvites(prev => prev.filter(inv => inv.id !== inviteId));
    } else {
      alert("Failed to reject the invite.");
    }
    setActionLoadingId(null);
  }

  async function handleSendInvite(e: React.FormEvent) {
    e.preventDefault();
    if (!emailInput.trim()) return;

    setSendLoading(true);
    const result = await coWorkerService.sendInvite({ email: emailInput });

    if (result.success) {
      setEmailInput('');
      const updatedSent = await coWorkerService.getSentInvites();
      setSentInvites(updatedSent);
    } else {
      alert(result.message || "Failed to send the invitation.");
    }
    setSendLoading(false);
  }

  if (pageLoading) {

    console.log("user para ver os nomes", user?.email)
    console.log("user para ver os nomes", user?.ownerInfo?.email)

    return (
      <div className="flex min-h-[400px] items-center justify-center text-white">
        <Loader2 className="w-8 h-8 animate-spin text-zinc-400" />
      </div>
    );
  }

  const TeamGraphComponent = () => (
    <Card className="bg-[#2A2A2A] border-zinc-800 text-white rounded-[25px] p-6">
      <h2 className="text-xl font-semibold mb-6 flex items-center gap-2">
        <Users className="w-5 h-5 text-[#A78BFA]" /> Team Hierarchy Graph
      </h2>

      <div className="flex flex-col items-center justify-center py-6 w-full overflow-x-auto">
        {/* Cabeçalho do Owner */}
        <div className="flex flex-col items-center relative z-10">
          <div className="bg-zinc-900 border-2 border-[#A78BFA] px-6 py-3 rounded-2xl flex items-center gap-3 shadow-xl min-w-[200px] justify-center">
            <Shield className="w-5 h-5 text-[#A78BFA] shrink-0" />
            <div className="text-center">
              <p className="text-xs text-zinc-400 font-medium uppercase tracking-wider">Workspace Owner</p>
              <p className="text-sm font-semibold truncate max-w-[160px]">
                {isOwner ? (user?.email || 'You') : (user?.ownerInfo?.email || 'Owner')}
              </p>
            </div>
          </div>

          {teamMembers.length > 0 && <div className="w-[2px] h-8 bg-zinc-700"></div>}
        </div>

        {teamMembers.length === 0 ? (
          <p className="text-zinc-500 text-xs italic mt-2">No active co-workers connected yet.</p>
        ) : (
          <div className="w-full max-w-2xl">
            {teamMembers.length > 1 && (
              <div className="relative w-full flex justify-center">
                <div className="absolute h-[2px] bg-zinc-700 top-0" style={{
                  width: `calc(100% - (${100 / teamMembers.length}%)`
                }}></div>
              </div>
            )}

            <div className="flex justify-center items-start gap-4 w-full pt-0">
              {teamMembers.map((worker) => {
                const isMe = worker.userId === user?.id;
                const canRemove = isOwner || isMe;

                return (
                  <div key={worker.id} className="flex flex-col items-center flex-1 min-w-[160px] max-w-[240px] relative mt-6">
                    <div className="w-[2px] h-6 bg-zinc-700 mb-0"></div>

                    {canRemove && (
                      <button
                        onClick={() => handleRemoveCoWorker(worker.userId)}
                        className="absolute -top-3 left-1/2 -translate-x-1/2 bg-red-900/80 hover:bg-red-600 rounded-full p-1 transition z-20 border border-red-700 shadow-md"
                      >
                        <X className="w-3 h-3 text-white" />
                      </button>
                    )}

                    <div className={`bg-[#1F1F1F] border px-4 py-3 rounded-xl flex items-center gap-3 w-full shadow-lg hover:border-zinc-700 transition ${isMe ? 'border-emerald-500/50' : 'border-zinc-800'}`}>
                      <div className="w-8 h-8 rounded-full bg-zinc-800 flex items-center justify-center shrink-0">
                        <UserCheck className={`w-4 h-4 ${isMe ? 'text-emerald-400 animate-pulse' : 'text-[#A78BFA]'}`} />
                      </div>
                      <div className="overflow-hidden">
                        <p className="text-xs text-emerald-400 font-semibold tracking-wide uppercase">
                          {isMe ? 'Co-Worker (You)' : 'Co-Worker'}
                        </p>
                        <p className="text-xs text-zinc-300 truncate font-medium">
                          Email: {worker.coWorkerEmail}
                        </p>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </Card>
  );

  return (
    <div className="text-white max-w-4xl mx-auto pt-[5vh] px-4 pb-12">
      {/* Cabeçalho Dinâmico */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold">
          {isOwner || isCoWorker ? "Team Management" : "Co-Worker Invites"}
        </h1>
        <p className="text-sm text-zinc-400 mt-2">
          {isOwner && "Invite new members to your workspace and track existing team invitations."}
          {isCoWorker && "View the workspace structure and your fellow team members."}
          {user?.role === 'justUser' && "Manage team invitations sent to your account. Accepting an invite connects you to a workspace."}
        </p>
      </div>

      <div className="w-full opacity-30 h-[1px] bg-zinc-500 mb-8"></div>

      {/* RENDERIZAÇÃO CONSOANTE A ROLE */}
      {isOwner && (
        <div className="space-y-8">
          <TeamGraphComponent />

          {/* Formulário para Enviar Convite */}
          <Card className="bg-[#2A2A2A] border-zinc-800 text-white rounded-[25px] p-6">
            <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <Send className="w-5 h-5 text-[#A78BFA]" /> Invite a Co-Worker
            </h2>
            <form onSubmit={handleSendInvite} className="flex flex-col sm:flex-row gap-3">
              <input
                type="email"
                placeholder="Enter co-worker's email address..."
                className="flex-1 bg-[#1A1A1A] border border-zinc-700 p-3 rounded-xl text-white outline-none focus:border-[#A78BFA]"
                value={emailInput}
                onChange={(e) => setEmailInput(e.target.value)}
                disabled={sendLoading}
                required
              />
              <Button
                type="submit"
                disabled={sendLoading}
                className="bg-[#A78BFA] hover:bg-[#8B5CF6] text-white px-6 h-12 rounded-xl gap-2 font-medium"
              >
                {sendLoading ? <Loader2 className="w-4 h-4 animate-spin" /> : <Send className="w-4 h-4" />}
                Send Invite
              </Button>
            </form>
          </Card>

          {/* Lista de Convites Enviados */}
          <div className="space-y-4">
            <h2 className="text-xl font-semibold flex items-center gap-2 text-zinc-300">
              <Clock className="w-5 h-5" /> Sent Invitations
            </h2>

            {sentInvites.length === 0 ? (
              <p className="text-zinc-500 text-sm italic pl-2">No invitations sent yet.</p>
            ) : (
              <div className="space-y-3">
                {sentInvites.map((invite) => (
                  <Card key={invite.id} className="bg-[#232323] border-zinc-800 text-white rounded-[15px]">
                    <CardContent className="p-4 flex items-center justify-between gap-4">
                      <div className="flex items-center gap-3">
                        <Mail className="w-5 h-5 text-zinc-500" />
                        <div>
                          <p className="text-sm font-medium text-zinc-200">{invite.coWorkerEmail}</p>
                          <p className="text-xs text-zinc-500">Sent on: {new Date(invite.createdAt).toLocaleDateString()}</p>
                        </div>
                      </div>

                      <span className={`text-xs px-3 py-1 rounded-full font-medium ${invite.status.toLowerCase() === 'accepted'
                        ? 'bg-emerald-950 text-emerald-400 border border-emerald-800'
                        : invite.status.toLowerCase() === 'rejected'
                          ? 'bg-red-950 text-red-400 border border-red-900'
                          : 'bg-zinc-800 text-zinc-400 border border-zinc-700'
                        }`}>
                        {invite.status.toUpperCase()}
                      </span>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {isCoWorker && (
        <div className="space-y-8">
          {/* Apenas o Grafo da Equipa aparece aqui */}
          <TeamGraphComponent />
        </div>
      )}

      {user?.role === 'justUser' && (
        <div className="space-y-4">
          {receivedInvites.length === 0 ? (
            <div className="text-center p-12 bg-[#2A2A2A] rounded-[25px] border border-zinc-800 text-zinc-400">
              <Mail className="w-12 h-12 mx-auto mb-3 text-zinc-600" />
              <p className="text-lg font-medium">No pending invitations</p>
              <p className="text-sm text-zinc-500 mt-1">You are all caught up!</p>
            </div>
          ) : (
            receivedInvites.map((invite) => {
              const isProcessing = actionLoadingId === invite.id;

              return (
                <Card
                  key={invite.id}
                  className="bg-[#2A2A2A] border-zinc-800 text-white rounded-[20px] overflow-hidden"
                >
                  <CardContent className="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div className="flex items-start gap-4">
                      <div className="w-12 h-12 bg-zinc-800 rounded-full flex items-center justify-center shrink-0">
                        <UserCheck className="w-6 h-6 text-[#A78BFA]" />
                      </div>
                      <div className="space-y-1">
                        <p className="text-base font-medium text-zinc-200">
                          Invitation to join as Co-Worker
                        </p>
                        <div className="flex items-center gap-2 text-xs text-zinc-400">
                          <span className="bg-zinc-800 px-2 py-0.5 rounded text-zinc-300 font-mono">
                            ID: #{invite.id}
                          </span>
                          <span>•</span>
                          <span>Received: {new Date(invite.createdAt).toLocaleDateString()}</span>
                        </div>
                      </div>
                    </div>

                    <div className="flex items-center gap-3 self-end sm:self-center">
                      <Button
                        variant="ghost"
                        onClick={() => handleReject(invite.id)}
                        disabled={isProcessing}
                        className="text-red-400 hover:text-red-300 hover:bg-red-950/30 border border-zinc-800 px-4 rounded-xl gap-2"
                      >
                        {isProcessing ? <Loader2 className="w-4 h-4 animate-spin" /> : <><X className="w-4 h-4" /> Reject</>}
                      </Button>

                      <Button
                        onClick={() => handleAccept(invite.id)}
                        disabled={isProcessing}
                        className="bg-[#A78BFA] hover:bg-[#8B5CF6] text-white px-4 rounded-xl gap-2"
                      >
                        {isProcessing ? <Loader2 className="w-4 h-4 animate-spin" /> : <><Check className="w-4 h-4" /> Accept</>}
                      </Button>
                    </div>
                  </CardContent>
                </Card>
              );
            })
          )}
        </div>
      )}
    </div>
  );
}