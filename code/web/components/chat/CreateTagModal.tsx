'use client';
import React, { useState } from 'react';
import { X } from 'lucide-react';

interface CreateTagModalProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: (tagName: string, colorHex: string) => void;
    isLoading?: boolean;
    title?: string;
    confirmLabel?: string;
    initialTagName?: string;
    initialColorHex?: string;
}

export const CreateTagModal = ({
    isOpen,
    onClose,
    onConfirm,
    isLoading = false,
    title = 'Create New Tag',
    confirmLabel = 'Create',
    initialTagName = '',
    initialColorHex = '#3b82f6'
}: CreateTagModalProps) => {
    const [tagName, setTagName] = useState(initialTagName);
    const [colorHex, setColorHex] = useState(initialColorHex);

    React.useEffect(() => {
        if (isOpen) {
            setTagName(initialTagName);
            setColorHex(initialColorHex);
        }
    }, [isOpen, initialTagName, initialColorHex]);

    const handleConfirm = () => {
        if (!tagName.trim()) {
            alert('Please enter a tag name');
            return;
        }
        onConfirm(tagName, colorHex);
        setTagName('');
        setColorHex('#3b82f6');
    };

    const handleCancel = () => {
        setTagName('');
        setColorHex('#3b82f6');
        onClose();
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-zinc-800 border border-zinc-700 rounded-lg shadow-xl w-96 p-6">
                <div className="flex items-center justify-between mb-4">
                    <h2 className="text-lg font-bold text-white">{title}</h2>
                    <button
                        onClick={handleCancel}
                        className="p-1 hover:bg-zinc-700 rounded transition-colors"
                    >
                        <X size={18} className="text-zinc-400" />
                    </button>
                </div>

                <div className="space-y-4">
                    {/* Tag Name Input */}
                    <div>
                        <label className="block text-xs font-semibold text-zinc-300 mb-2 uppercase">
                            Tag Name
                        </label>
                        <input
                            type="text"
                            value={tagName}
                            onChange={(e) => setTagName(e.target.value)}
                            placeholder="Enter tag name"
                            className="w-full px-3 py-2 bg-zinc-700 border border-zinc-600 rounded text-white placeholder-zinc-500 focus:outline-none focus:border-blue-500 transition-colors"
                            onKeyDown={(e) => {
                                if (e.key === 'Enter') handleConfirm();
                            }}
                            autoFocus
                        />
                    </div>

                    {/* Color Picker */}
                    <div>
                        <label className="block text-xs font-semibold text-zinc-300 mb-2 uppercase">
                            Color
                        </label>
                        <div className="flex items-center gap-3">
                            <input
                                type="color"
                                value={colorHex}
                                onChange={(e) => setColorHex(e.target.value)}
                                className="w-16 h-10 cursor-pointer border border-zinc-600 rounded"
                            />
                            <div
                                className="flex-1 h-10 rounded border border-zinc-600 flex items-center justify-center font-mono text-sm text-zinc-300"
                                style={{ backgroundColor: colorHex }}
                            >
                                {colorHex}
                            </div>
                        </div>
                    </div>

                    {/* Preview */}
                    <div>
                        <label className="block text-xs font-semibold text-zinc-300 mb-2 uppercase">
                            Preview
                        </label>
                        <span
                            className="inline-block px-3 py-1 rounded text-xs font-bold uppercase tracking-tighter text-white"
                            style={{ backgroundColor: colorHex }}
                        >
                            {tagName || 'Tag Name'}
                        </span>
                    </div>
                </div>

                {/* Actions */}
                <div className="flex gap-2 mt-6">
                    <button
                        onClick={handleCancel}
                        className="flex-1 px-4 py-2 bg-zinc-700 hover:bg-zinc-600 text-white rounded transition-colors font-medium"
                        disabled={isLoading}
                    >
                        Cancel
                    </button>
                    <button
                        onClick={handleConfirm}
                        className="flex-1 px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
                        disabled={isLoading || !tagName.trim()}
                    >
                        {isLoading ? 'Creating...' : 'Create'}
                    </button>
                </div>
            </div>
        </div>
    );
};
