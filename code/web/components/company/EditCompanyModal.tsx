'use client';
import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Loader2 } from 'lucide-react';
import companyService from '@/services/api/CompanyService'; 
import usersService from '@/services/api/UsersService';
import sectorService, { Sector } from '@/services/api/SectorService'; 
import { CompanyData } from '@/services/interfaces/ICompanyService';
import { EditCompanyModalProps } from '@/services/DTO/company/EditCompanyModalProsInputModel';

export function EditCompanyModal({ currentUsername, initialData, onClose, onSuccess }: EditCompanyModalProps) {
  const [username, setUsername] = useState(currentUsername);
  const [formData, setFormData] = useState<CompanyData>({
    companyName: initialData?.companyName || '',
    companySize: initialData?.companySize || 0,
    description: initialData?.description || '',
    sectors: initialData?.sectors || [], // Lista de strings
    websiteLink: initialData?.websiteLink || '',
    countryHeadquarters: initialData?.countryHeadquarters || ''
  });

  const [availableSectors, setAvailableSectors] = useState<Sector[]>([]);
  const [loadingSectors, setLoadingSectors] = useState(true);
  const [loading, setLoading] = useState(false);

  // 1. Carregar setores disponíveis ao abrir o modal
  useEffect(() => {
    async function fetchSectors() {
      try {
        const data = await sectorService.getAllSectors();
        setAvailableSectors(data);
      } catch (err) {
        console.error("Erro ao carregar setores:", err);
      } finally {
        setLoadingSectors(false);
      }
    }
    fetchSectors();
  }, []);

  // 2. Lógica de seleção de setores (Check/Uncheck)
  const handleSectorChange = (sectorName: string) => {
    setFormData(prev => {
      const isSelected = prev.sectors.includes(sectorName);
      if (isSelected) {
        return { ...prev, sectors: prev.sectors.filter(s => s !== sectorName) };
      } else {
        return { ...prev, sectors: [...prev.sectors, sectorName] };
      }
    });
  };

  const handleSave = async () => {
    setLoading(true);
    try {
      const results = await Promise.all([
        usersService.editUsername(username),
        companyService.editCompany({
          ...formData,
          companySize: Number(formData.companySize)
        })
      ]);

      if (results[0] && results[1]) {
        onSuccess();
        onClose();
      } else {
        alert("Erro ao atualizar dados.");
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-[#2A2A2A] p-8 rounded-[25px] w-full max-w-lg border border-zinc-700 text-white max-h-[90vh] overflow-y-auto shadow-2xl">
        <h2 className="text-xl font-bold mb-6 border-b border-zinc-800 pb-2">Edit Company Profile</h2>

        <div className="space-y-6">
          {/* Username */}
          <div className="space-y-2">
            <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Account Username</label>
            <input 
              className="w-full bg-[#1A1A1A] border border-zinc-700 p-3 rounded-xl text-white outline-none focus:border-purple-500 transition-all"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>

          <div className="grid grid-cols-1 gap-4">
            {/* Nome e Tamanho */}
            <div className="grid grid-cols-2 gap-4">
                <div className="space-y-1">
                    <label className="text-[10px] text-zinc-400 uppercase font-bold">Company Name</label>
                    <input 
                        className="w-full bg-zinc-900 border border-zinc-700 p-3 rounded-xl outline-none focus:border-purple-500"
                        value={formData.companyName}
                        onChange={(e) => setFormData({...formData, companyName: e.target.value})}
                    />
                </div>
                <div className="space-y-1">
                    <label className="text-[10px] text-zinc-400 uppercase font-bold">Company Size</label>
                    <input 
                        type="number"
                        className="w-full bg-zinc-900 border border-zinc-700 p-3 rounded-xl outline-none focus:border-purple-500"
                        value={formData.companySize}
                        onChange={(e) => setFormData({...formData, companySize: e.target.value})}
                    />
                </div>
            </div>

            {/* SELECÇÃO DE SETORES */}
            <div className="space-y-2">
              <label className="text-[10px] text-zinc-400 uppercase font-bold">Business Sectors</label>
              <div className="bg-zinc-900 border border-zinc-700 rounded-xl p-4 max-h-40 overflow-y-auto space-y-2">
                {loadingSectors ? (
                  <div className="flex justify-center py-2"><Loader2 className="animate-spin w-4 h-4 text-zinc-500" /></div>
                ) : (
                  availableSectors.map((sector) => (
                    <div key={sector.id} className="flex items-center space-x-3">
                      <Checkbox 
                        id={`edit-sector-${sector.id}`} 
                        checked={formData.sectors.includes(sector.name)}
                        onCheckedChange={() => handleSectorChange(sector.name)}
                        className="border-zinc-500 data-[state=checked]:bg-purple-600 data-[state=checked]:border-purple-600"
                      />
                      <Label 
                        htmlFor={`edit-sector-${sector.id}`} 
                        className="text-sm font-normal cursor-pointer text-zinc-300 hover:text-white"
                      >
                        {sector.name}
                      </Label>
                    </div>
                  ))
                )}
              </div>
            </div>

            <div className="space-y-1">
              <label className="text-[10px] text-zinc-400 uppercase font-bold">Website</label>
              <input 
                className="w-full bg-zinc-900 border border-zinc-700 p-3 rounded-xl outline-none focus:border-purple-500"
                value={formData.websiteLink}
                onChange={(e) => setFormData({...formData, websiteLink: e.target.value})}
              />
            </div>

            <div className="space-y-1">
              <label className="text-[10px] text-zinc-400 uppercase font-bold">Description</label>
              <textarea 
                className="w-full bg-zinc-900 border border-zinc-700 p-3 rounded-xl outline-none focus:border-purple-500 h-20 resize-none"
                value={formData.description}
                onChange={(e) => setFormData({...formData, description: e.target.value})}
              />
            </div>
          </div>
        </div>

        <div className="flex gap-4 mt-8">
          <Button variant="ghost" className="flex-1 text-zinc-400" onClick={onClose}>Cancel</Button>
          <Button 
            className="flex-1 bg-purple-600 hover:bg-purple-700 text-white font-bold" 
            onClick={handleSave}
            disabled={loading}
          >
            {loading ? <Loader2 className="animate-spin w-4 h-4" /> : "Save Changes"}
          </Button>
        </div>
      </div>
    </div>
  );
}