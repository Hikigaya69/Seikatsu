import { useState, useEffect } from "react";
import api from "../../Utils/api";
import { Trash2, Pencil, CheckCircle, XCircle, X } from "lucide-react";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function ProductTable({ data, refresh }) {
    const [categories, setCategories] = useState([]);
    const [confirmId, setConfirmId] = useState(null);
    const [errorMsg, setErrorMsg] = useState("");
    const [toast, setToast] = useState(null); // { message, type: "success" | "error" }

    const [editProduct, setEditProduct] = useState(null);
    const [editForm, setEditForm] = useState({});
    const [editPreview, setEditPreview] = useState("");

    // ── Auto-dismiss toast after 3s ─────────────────────────
    useEffect(() => {
        if (!toast) return;
        const timer = setTimeout(() => setToast(null), 3000);
        return () => clearTimeout(timer);
    }, [toast]);

    // ── Fetch categories once ───────────────────────────────
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const res = await api.get("/Admin/getallcats");
                setCategories(res.data.data);
            } catch (err) {
                console.error("Failed to fetch categories", err);
            }
        };
        fetchCategories();
    }, []);

    // ── Delete ──────────────────────────────────────────────
    const deleteProduct = async (id) => {
        try {
            await api.delete(`/Admin/deleteproduct/${id}`);
            refresh();
        } catch (err) {
            const status = err?.response?.status;
            setErrorMsg(
                status === 500
                    ? "This product is in one or more carts and cannot be deleted."
                    : "Failed to delete product. Please try again."
            );
        } finally {
            setConfirmId(null);
        }
    };

    // ── Edit open ───────────────────────────────────────────
    const openEdit = (product) => {
        setEditProduct(product);
        setEditForm({
            name: product.name,
            description: product.description,
            price: product.price,
            countryName: product.countryName,
            storageType: product.storageType,
            categoryId: product.categoryId,
            isFood: product.isFood,
            image: null,
        });
        setEditPreview(product.productImageUrl);
    };

    const handleEditImage = (e) => {
        const file = e.target.files[0];
        if (!file) return;
        setEditForm({ ...editForm, image: file });
        setEditPreview(URL.createObjectURL(file));
    };

    // ── Edit submit ─────────────────────────────────────────
    const handleEditSubmit = async () => {
    // ── Validate before sending ─────────────────────────
    if (!editForm.name?.trim()) {
        setToast({ message: "Product name is required.", type: "error" });
        return;
    }
    if (!editForm.price || isNaN(editForm.price)) {
        setToast({ message: "A valid price is required.", type: "error" });
        return;
    }
    if (!editForm.categoryId) {
        setToast({ message: "Please select a category.", type: "error" });
        return;
    }
    if (!editForm.countryName?.trim()) {
        setToast({ message: "Country name is required.", type: "error" });
        return;
    }
    if (!editForm.storageType?.trim()) {
        setToast({ message: "Storage type is required.", type: "error" });
        return;
    }

    try {
        const formData = new FormData();
        formData.append("Name", editForm.name.trim());
        formData.append("Description", editForm.description?.trim() ?? "");
        formData.append("Price", Number(editForm.price));        // ← cast to number
        formData.append("CountryName", editForm.countryName.trim());
        formData.append("StorageType", editForm.storageType.trim());
        formData.append("CategoryId", editForm.categoryId);      // ← must be a valid GUID
        formData.append("IsFood", editForm.isFood ?? true);
        if (editForm.image) {
            formData.append("Image", editForm.image);
        }

        await api.patch(`/Admin/editproduct/${editProduct.id}`, formData);

        setToast({ message: `${editForm.name} updated successfully.`, type: "success" });
        refresh();
        setEditProduct(null);

    } catch (err) {
        console.error(err);
        const status = err?.response?.status;
        setToast({
            message: status === 400
                ? "Invalid data. Please check all fields."
                : "Failed to update product. Please try again.",
            type: "error"
        });
    }
};
    return (
        <div className="bg-white rounded-2xl shadow overflow-hidden">

            {/* ── Toast ── */}
            {toast && (
                <div className={`fixed bottom-6 right-6 z-50 flex items-center gap-3 px-4 py-3 rounded-xl shadow-lg text-white text-sm
          ${toast.type === "success" ? "bg-green-500" : "bg-red-500"}`}
                >
                    {toast.type === "success"
                        ? <CheckCircle size={18} />
                        : <XCircle size={18} />
                    }
                    <span>{toast.message}</span>
                    <button onClick={() => setToast(null)} className="ml-2 hover:opacity-70">
                        <X size={16} />
                    </button>
                </div>
            )}

            {/* ── Error Banner ── */}
            {errorMsg && (
                <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 text-sm flex justify-between">
                    <span>{errorMsg}</span>
                    <button onClick={() => setErrorMsg("")} className="font-bold ml-4">✕</button>
                </div>
            )}

            {/* ── Delete Confirm Dialog ── */}
            {confirmId && (
                <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
                    <div className="bg-white rounded-2xl shadow-xl p-6 w-80 text-center">
                        <p className="font-semibold text-gray-800 mb-2">Delete Product?</p>
                        <p className="text-sm text-gray-500 mb-6">
                            This action is permanent. If the product is in any cart, deletion will fail.
                        </p>
                        <div className="flex gap-3 justify-center">
                            <button
                                onClick={() => setConfirmId(null)}
                                className="px-4 py-2 rounded-lg border text-gray-600 hover:bg-gray-100"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={() => deleteProduct(confirmId)}
                                className="px-4 py-2 rounded-lg bg-red-500 text-white hover:bg-red-600"
                            >
                                Delete
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* ── Edit Dialog ── */}
            <Dialog open={!!editProduct} onOpenChange={(open) => !open && setEditProduct(null)}>
                <DialogContent className="max-w-2xl">
                    <DialogHeader>
                        <DialogTitle>Edit Product</DialogTitle>
                    </DialogHeader>

                    <div className="grid grid-cols-2 gap-5">
                        <Input
                            placeholder="Product name"
                            value={editForm.name || ""}
                            onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
                        />
                        <Input
                            placeholder="Price"
                            type="number"
                            value={editForm.price || ""}
                            onChange={(e) => setEditForm({ ...editForm, price: e.target.value })}
                        />
                        <Input
                            placeholder="Country"
                            value={editForm.countryName || ""}
                            onChange={(e) => setEditForm({ ...editForm, countryName: e.target.value })}
                        />
                        <Input
                            placeholder="Storage Type"
                            value={editForm.storageType || ""}
                            onChange={(e) => setEditForm({ ...editForm, storageType: e.target.value })}
                        />

                        <select
                            className="border rounded-lg px-3 h-10"
                            value={editForm.categoryId || ""}
                            onChange={(e) => setEditForm({ ...editForm, categoryId: e.target.value })}
                        >
                            <option value="">Select Category</option>
                            {categories.map((category) => (
                                <option key={category.id} value={category.id}>
                                    {category.categoryName}
                                </option>
                            ))}
                        </select>

                        <label className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-xl h-24 cursor-pointer hover:border-blue-400 hover:bg-blue-50 transition-colors group">
                            <div className="flex flex-col items-center text-gray-400 group-hover:text-blue-400 transition-colors">
                                <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 mb-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16v1a2 2 0 002 2h12a2 2 0 002-2v-1M12 12V4m0 0L8 8m4-4l4 4" />
                                </svg>
                                <span className="text-xs font-medium">Click to upload image</span>
                                <span className="text-[10px] text-gray-300 mt-0.5">PNG, JPG, WEBP</span>
                            </div>
                            <input type="file" className="hidden" onChange={handleEditImage} accept="image/*" />
                        </label>
                    </div>

                    <textarea
                        placeholder="Description"
                        className="w-full border rounded-lg p-3 mt-4"
                        rows={4}
                        value={editForm.description || ""}
                        onChange={(e) => setEditForm({ ...editForm, description: e.target.value })}
                    />

                    {editPreview && (
                        <div className="relative w-40 h-40 mt-4 group">
                            <img
                                src={editPreview}
                                className="w-40 h-40 object-cover rounded-xl shadow"
                                alt="Preview"
                            />
                            <div className="absolute inset-0 bg-black/40 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                                <button
                                    onClick={() => {
                                        setEditPreview("");
                                        setEditForm({ ...editForm, image: null });
                                    }}
                                    className="text-white bg-red-500 hover:bg-red-600 rounded-full p-1.5"
                                >
                                    <Trash2 size={14} />
                                </button>
                            </div>
                            <p className="text-xs text-gray-400 mt-1 truncate w-40">
                                {editForm.image?.name ?? "Current image"}
                            </p>
                        </div>
                    )}

                    <Button onClick={handleEditSubmit} className="w-full mt-5">
                        Save Changes
                    </Button>
                </DialogContent>
            </Dialog>

            {/* ── Table Header ── */}
            <div className="grid grid-cols-6 bg-gray-100 p-4 font-semibold text-sm">
                <p>Product</p>
                <p>Category</p>
                <p>Country</p>
                <p>Price</p>
                <p>Created</p>
                <p>Actions</p>
            </div>

            {/* ── Rows ── */}
            {data.map((product) => (
                <div
                    key={product.id}
                    className="grid grid-cols-6 items-center p-4 border-b hover:bg-gray-50 transition"
                >
                    <div className="flex items-center gap-3">
                        <img
                            src={product.productImageUrl}
                            alt={product.name}
                            className="w-14 h-14 rounded-xl object-cover"
                        />
                        <div>
                            <p className="font-medium">{product.name}</p>
                            <p className="text-xs text-gray-400">{product.description?.slice(0, 40)}</p>
                        </div>
                    </div>
                    <p>{product.category}</p>
                    <p>{product.countryName}</p>
                    <p>₹{product.price}</p>
                    <p className="text-sm text-gray-500">
                        {new Date(product.createdAt).toLocaleDateString()}
                    </p>
                    <div className="flex gap-4">
                        <button
                            onClick={() => openEdit(product)}
                            className="text-gray-500 hover:text-black"
                        >
                            <Pencil size={18} />
                        </button>
                        <button
                            onClick={() => setConfirmId(product.id)}
                            className="text-red-500 hover:text-red-700"
                        >
                            <Trash2 size={18} />
                        </button>
                    </div>
                </div>
            ))}
        </div>
    );
}