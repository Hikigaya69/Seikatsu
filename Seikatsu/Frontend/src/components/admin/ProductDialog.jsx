import { useState, useEffect } from "react";
import api from "../../Utils/api";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Trash2, CheckCircle, X } from "lucide-react";

export default function ProductDialog({ categories, refresh }) {
    const [open, setOpen] = useState(false);
    const [toast, setToast] = useState(false);
    const [preview, setPreview] = useState("");
    const [form, setForm] = useState({
        name: "",
        description: "",
        price: "",
        countryName: "",
        storageType: "",
        categoryId: "",
        isFood: true,
        image: null
    });

    useEffect(() => {
        if (!toast) return;
        const timer = setTimeout(() => setToast(false), 3000);
        return () => clearTimeout(timer);
    }, [toast]);

    const handleImage = (e) => {
        const file = e.target.files[0];
        if (!file) return;
        setForm({ ...form, image: file });
        setPreview(URL.createObjectURL(file));
    };

    const handleSubmit = async () => {
        try {
            const formData = new FormData();
            formData.append("Name", form.name);
            formData.append("Description", form.description);
            formData.append("Price", form.price);
            formData.append("CountryName", form.countryName);
            formData.append("StorageType", form.storageType);
            formData.append("CategoryId", form.categoryId);
            formData.append("IsFood", form.isFood);
            formData.append("Image", form.image);
            await api.post("/Admin/adminaddproducts", formData);
            refresh();
            setOpen(false);   // close dialog
            setToast(true);   // show toast
        } catch (err) {
            console.error(err);
        }
    };

    return (
        <>
            {/* Toast */}
            {toast && (
                <div className="fixed bottom-6 right-6 z-50 flex items-center gap-3 px-4 py-3 rounded-xl shadow-lg bg-green-500 text-white text-sm">
                    <CheckCircle size={18} />
                    <span>Product added successfully!</span>
                    <button onClick={() => setToast(false)} className="ml-2 hover:opacity-70">
                        <X size={16} />
                    </button>
                </div>
            )}

            <Dialog open={open} onOpenChange={setOpen}>
                <DialogTrigger asChild>
                    <Button>Add Product</Button>
                </DialogTrigger>

                <DialogContent className="max-w-2xl">
                    <DialogHeader>
                        <DialogTitle>Add New Product</DialogTitle>
                    </DialogHeader>

                    <div className="grid grid-cols-2 gap-5">
                        <Input
                            placeholder="Product name"
                            value={form.name}
                            onChange={(e) => setForm({ ...form, name: e.target.value })}
                        />
                        <Input
                            placeholder="Price"
                            type="number"
                            value={form.price}
                            onChange={(e) => setForm({ ...form, price: e.target.value })}
                        />
                        <Input
                            placeholder="Country"
                            value={form.countryName}
                            onChange={(e) => setForm({ ...form, countryName: e.target.value })}
                        />
                        <Input
                            placeholder="Storage Type"
                            value={form.storageType}
                            onChange={(e) => setForm({ ...form, storageType: e.target.value })}
                        />

                        <select
                            className="border rounded-lg px-3 h-10"
                            value={form.categoryId}
                            onChange={(e) => setForm({ ...form, categoryId: e.target.value })}
                        >
                            <option value="">Select Category</option>
                            {categories.map((category) => (
                                <option key={category.id} value={category.id}>
                                    {category.categoryName}
                                </option>
                            ))}
                        </select>

                        {/* Enhanced Upload */}
                        <label className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-xl h-24 cursor-pointer hover:border-blue-400 hover:bg-blue-50 transition-colors group">
                            <div className="flex flex-col items-center text-gray-400 group-hover:text-blue-400 transition-colors">
                                <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 mb-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16v1a2 2 0 002 2h12a2 2 0 002-2v-1M12 12V4m0 0L8 8m4-4l4 4" />
                                </svg>
                                <span className="text-xs font-medium">Click to upload image</span>
                                <span className="text-[10px] text-gray-300 mt-0.5">PNG, JPG, WEBP</span>
                            </div>
                            <input type="file" className="hidden" onChange={handleImage} accept="image/*" />
                        </label>
                    </div>

                    <textarea
                        placeholder="Description"
                        className="w-full border rounded-lg p-3 mt-4"
                        rows={4}
                        value={form.description}
                        onChange={(e) => setForm({ ...form, description: e.target.value })}
                    />

                    {/* Enhanced Preview */}
                    {preview && (
                        <div className="relative w-40 h-40 mt-4 group">
                            <img
                                src={preview}
                                className="w-40 h-40 object-cover rounded-xl shadow"
                                alt="Preview"
                            />
                            <div className="absolute inset-0 bg-black/40 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                                <button
                                    onClick={() => {
                                        setPreview("");
                                        setForm({ ...form, image: null });
                                    }}
                                    className="text-white bg-red-500 hover:bg-red-600 rounded-full p-1.5"
                                >
                                    <Trash2 size={14} />
                                </button>
                            </div>
                            <p className="text-xs text-gray-400 mt-1 truncate w-40">
                                {form.image?.name}
                            </p>
                        </div>
                    )}

                    <Button onClick={handleSubmit} className="w-full mt-5">
                        Save Product
                    </Button>
                </DialogContent>
            </Dialog>
        </>
    );
}