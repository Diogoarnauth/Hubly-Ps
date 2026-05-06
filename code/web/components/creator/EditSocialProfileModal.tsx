'use client';
import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Loader2, X } from 'lucide-react';
import creatorService from '@/services/api/CreatorService';
import sectorService, { Sector } from '@/services/api/SectorService';
import { toastError, toastSuccess } from '../ToastImplementations';
import  EditSocialProfileModalProps from '@/services/DTO/creator/EditSocialProfileModalPropsInputModel';

export function EditSocialProfileModal({ initialData, onClose, onSuccess }: EditSocialProfileModalProps) {
    const [formData, setFormData] = useState({
        platformUserName: initialData.platformUserName || '',
        link: initialData.link || '',
        description: initialData.description || '',
        followersCount: initialData.followersCount || 0,
        priceMin: initialData.priceMin || 0,
        priceMax: initialData.priceMax || 0,
        sectors: initialData.sectors || [] as string[]
    });

    const [availableSectors, setAvailableSectors] = useState<Sector[]>([]);
    const [loadingSectors, setLoadingSectors] = useState(true);
    const [loading, setLoading] = useState(false);

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
            const response = await creatorService.editSocialProfile(initialData.id, {
                ...formData,
                followersCount: Number(formData.followersCount),
                priceMin: Number(formData.priceMin),
                priceMax: Number(formData.priceMax)
            });

            if (response) {
                toastSuccess('Success', 'Social profile updated successfully');
                onSuccess();
                onClose();
            }
        } catch (error) {
            console.error(error);
            toastError('Error', 'Failed to update social profile');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <div className="bg-[#1A1A1A] p-8 rounded-[30px] w-full max-w-2xl border border-zinc-800 text-white max-h-[90vh] overflow-y-auto shadow-2xl relative">

                <button onClick={onClose} className="absolute top-6 right-6 text-zinc-500 hover:text-white transition-colors">
                    <X size={24} />
                </button>

                <h2 className="text-2xl font-bold mb-2">Edit {initialData.platformName} Profile</h2>
                <p className="text-zinc-500 text-sm mb-8 uppercase tracking-widest font-semibold">Update your social metrics and details</p>

                <div className="space-y-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        {/* Username na Plataforma */}
                        <div className="space-y-2">
                            <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Platform @Username</label>
                            <input
                                className="w-full bg-zinc-900 border border-zinc-800 p-3 rounded-xl outline-none focus:border-purple-500 transition-all"
                                value={formData.platformUserName}
                                onChange={(e) => setFormData({ ...formData, platformUserName: e.target.value })}
                            />
                        </div>

                        {/* Link */}
                        <div className="space-y-2">
                            <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Profile Link</label>
                            <input
                                className="w-full bg-zinc-900 border border-zinc-800 p-3 rounded-xl outline-none focus:border-purple-500 transition-all"
                                value={formData.link}
                                onChange={(e) => setFormData({ ...formData, link: e.target.value })}
                            />
                        </div>

                        {/* Seguidores */}
                        <div className="space-y-2">
                            <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Followers Count</label>
                            <input
                                type="number"
                                className="w-full bg-zinc-900 border border-zinc-800 p-3 rounded-xl outline-none focus:border-purple-500 transition-all"
                                value={formData.followersCount}
                                onChange={(e) => setFormData({ ...formData, followersCount: parseInt(e.target.value) || 0 })}
                            />
                        </div>

                        {/* Preços */}
                        <div className="grid grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Min Price (€)</label>
                                <input
                                    type="number"
                                    className="w-full bg-zinc-900 border border-zinc-800 p-3 rounded-xl outline-none focus:border-purple-500 transition-all"
                                    value={formData.priceMin}
                                    onChange={(e) => setFormData({ ...formData, priceMin: parseFloat(e.target.value) || 0 })}
                                />
                            </div>
                            <div className="space-y-2">
                                <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Max Price (€)</label>
                                <input
                                    type="number"
                                    className="w-full bg-zinc-900 border border-zinc-800 p-3 rounded-xl outline-none focus:border-purple-500 transition-all"
                                    value={formData.priceMax}
                                    onChange={(e) => setFormData({ ...formData, priceMax: parseFloat(e.target.value) || 0 })}
                                />
                            </div>
                        </div>
                    </div>

                    {/* Descrição */}
                    <div className="space-y-2">
                        <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Profile Bio / Description</label>
                        <textarea
                            className="w-full bg-zinc-900 border border-zinc-800 p-4 rounded-xl outline-none focus:border-purple-500 h-24 resize-none"
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                        />
                    </div>

                    {/* Setores */}
                    <div className="space-y-3">
                        <label className="text-[10px] text-zinc-400 uppercase font-bold tracking-widest">Content Sectors</label>
                        <div className="bg-zinc-900/50 border border-zinc-800 rounded-2xl p-4 grid grid-cols-2 gap-3 max-h-48 overflow-y-auto">
                            {loadingSectors ? (
                                <div key="loader-sectors" className="col-span-2 flex justify-center py-4">
                                    <Loader2 className="animate-spin w-5 h-5 text-purple-500" />
                                </div>
                            ) : (
                                availableSectors.map((sector) => (
                                    <div key={sector.id || sector.Id} className="flex items-center space-x-3 group">
                                        <Checkbox
                                            id={`edit-social-sector-${sector.id || sector.Id}`}
                                            checked={formData.sectors.includes(sector.name || sector.Name)}
                                            onCheckedChange={() => handleSectorChange(sector.name || sector.Name)}
                                            className="border-zinc-700 data-[state=checked]:bg-purple-600"
                                        />
                                        <Label
                                            htmlFor={`edit-social-sector-${sector.id || sector.Id}`}
                                            className="text-sm font-medium cursor-pointer text-zinc-400 group-hover:text-white transition-colors"
                                        >
                                            {sector.name || sector.Name}
                                        </Label>
                                    </div>
                                ))
                            )}
                        </div>
                    </div>
                </div>

                <div className="flex gap-4 mt-10">
                    <Button variant="ghost" className="flex-1 text-zinc-400 hover:bg-zinc-900" onClick={onClose}>Cancel</Button>
                    <Button
                        className="flex-1 bg-purple-600 hover:bg-purple-700 text-white font-bold h-12 rounded-xl"
                        onClick={handleSave}
                        disabled={loading}
                    >
                        {loading ? <Loader2 className="animate-spin w-5 h-5" /> : "Update Profile"}
                    </Button>
                </div>
            </div>
        </div>
    );
}