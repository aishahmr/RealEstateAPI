# model.py (fully corrected)

import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler, OneHotEncoder
from sklearn.compose import ColumnTransformer
from sklearn.linear_model import LinearRegression
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics import mean_absolute_error, r2_score
import joblib

# Load dataset
df = pd.read_csv("C:\\Users\\DELL\\Downloads\\RealEstateAPI-main-1\\RealEstateAPI-main\\RealEstateAPI\ML\EstimatePrice (2)\\EstimatePrice\FinalDataset.csv")

# Prepare numeric columns
df['Price 2023 (EGP)'] = df['Price 2023 (EGP)'].str.replace(',', '').astype(float)
df['Price 2024 (EGP)'] = df['Price 2024 (EGP)'].str.replace(',', '').astype(float)
df['Price 2025 (EGP)'] = df['Price 2025 (EGP)'].str.replace(',', '').astype(float)

# Fill missing text fields
df['Amenities'] = df['Amenities'].fillna("")
df['Nearby Facility'] = df['Nearby Facility'].fillna("")

# Encode Property Type
df['Property Type Encoded'] = df['Property Type'].map({'Apartment': 0, 'Villa': 1})

# Simulate realistic future prices
np.random.seed(42)
growth_2024 = np.random.uniform(0.1, 0.3, size=len(df))
growth_2025 = np.random.uniform(0.1, 0.25, size=len(df))
growth_2026 = np.random.uniform(0.1, 0.25, size=len(df))

df['Price 2024 (EGP)'] = df['Price 2023 (EGP)'] * (1 + growth_2024)
df['Price 2025 (EGP)'] = df['Price 2024 (EGP)'] * (1 + growth_2025)
df['Price 2026 (EGP)'] = df['Price 2025 (EGP)'] * (1 + growth_2026)

# Features and target
features = [
    'Price 2023 (EGP)', 'Price 2024 (EGP)', 'Price 2025 (EGP)',
    'Area (sqm)', 'Bedrooms', 'Bathrooms', 'Property Type Encoded',
    'Amenities', 'Nearby Facility'
]

X = df[features]
y = df['Price 2026 (EGP)']

# Preprocessing
numerical_features = ['Price 2023 (EGP)', 'Price 2024 (EGP)', 'Price 2025 (EGP)', 'Area (sqm)', 'Bedrooms', 'Bathrooms']
categorical_features = ['Property Type Encoded']
text_features = ['Amenities', 'Nearby Facility']

preprocessor = ColumnTransformer([
    ("num", StandardScaler(), numerical_features),
    ("cat", OneHotEncoder(handle_unknown="ignore"), categorical_features),
    ("amenities", TfidfVectorizer(), 'Amenities'),
    ("facilities", TfidfVectorizer(), 'Nearby Facility')
])

# Create a Pipeline
model = Pipeline([
    ("preprocessor", preprocessor),
    ("regressor", LinearRegression())
])

# Train-test split
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Train model
model.fit(X_train, y_train)

# Predict
y_pred = model.predict(X_test)

# Evaluate
mae = mean_absolute_error(y_test, y_pred)
r2 = r2_score(y_test, y_pred)

# Save model
joblib.dump(model, "FinalDataset.pkl")

print("✅ Model saved to: FinalDataset.pkl")
print("📊 Mean Absolute Error (MAE):", round(mae, 2))
print("📈 R² Score:", round(r2, 4))
