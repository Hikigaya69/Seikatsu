

import { useEffect, useState } from "react";
import api from "../../Utils/api";
import ProductTable from "../../components/admin/ProductTable";
import ProductDialog from "../../components/admin/ProductDialog";
import { Input } from "@/components/ui/input";

export default function Products() {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [search, setSearch] = useState("");
    const [selectedCategoryId, setSelectedCategoryId] = useState("");
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        fetchCategories();
        fetchProducts("");
    }, []);

    const fetchProducts = async (categoryId) => {
        setLoading(true);
        try {
            const params = { pageSize: 10000 };
            if (categoryId) params.categoryId = categoryId;

            const res = await api.get("/Admin/adminfilter", { params });
            setProducts(res.data.data.items || []);
        } catch (err) {
            if (err.response?.status === 404) {
                setProducts([]);
            } else {
                console.error(err);
            }
        } finally {
            setLoading(false);
        }
    };

    const fetchCategories = async () => {
        try {
            const res = await api.get("/Admin/getallcats");
            setCategories(res.data.data);
        } catch (err) {
            console.error(err);
        }
    };

    const handleCategoryChange = (e) => {
        const categoryId = e.target.value;
        setSelectedCategoryId(categoryId);
        fetchProducts(categoryId);
    };

    const filteredProducts = products.filter((product) =>
        product.name.toLowerCase().includes(search.toLowerCase())
    );

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-3xl font-bold">Products</h1>
                    <p className="text-gray-500 mt-1">Manage store inventory</p>
                </div>
                <ProductDialog
                    categories={categories}
                    refresh={() => fetchProducts(selectedCategoryId)}
                />
            </div>

            <div className="flex gap-4">
                <Input
                    placeholder="Search product..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="max-w-sm"
                />
                <select
                    value={selectedCategoryId}
                    onChange={handleCategoryChange}
                    className="border rounded-lg px-3 py-2 text-sm"
                >
                    <option value="">All Categories</option>
                    {categories.map((category) => (
                        <option key={category.id} value={category.id}>
                            {category.categoryName}
                        </option>
                    ))}
                </select>
            </div>

            {loading ? (
                <div className="flex justify-center items-center py-20 text-gray-400 text-sm">
                    Loading products...
                </div>
            ) : filteredProducts.length === 0 ? (
                <div className="flex justify-center items-center py-20 text-gray-400 text-sm">
                    No products found.
                </div>
            ) : (
                <ProductTable
                    data={filteredProducts}
                    refresh={() => fetchProducts(selectedCategoryId)}
                />
            )}
        </div>
    );
}
